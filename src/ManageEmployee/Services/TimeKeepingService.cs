using System.Net;
using Common.Models;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.DataTransferObject.TimeKeeperModels;
using ManageEmployee.Entities;
using ManageEmployee.Entities.InOutEntities;
using ManageEmployee.Services.Interfaces.Configurations;
using ManageEmployee.Services.Interfaces.FaceRecognitions;
using ManageEmployee.Services.Interfaces.Targets;
using ManageEmployee.Services.Interfaces.TimeKeepings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace ManageEmployee.Services;

public class TimeKeepingService : ITimeKeepingService
{
    private readonly IConfigurationService _configurationService;
    private readonly ApplicationDbContext _context;
    private readonly IFaceRecognitionService _faceRecognitionService;
    private readonly ITargetService _targetService;

    public TimeKeepingService(IConfigurationService configurationService, ApplicationDbContext context, 
        IFaceRecognitionService faceRecognitionService , 
        ITargetService targetService)
    {
        _configurationService = configurationService;
        _context = context;
        _faceRecognitionService = faceRecognitionService;
        _targetService = targetService;
    }

    public async Task<List<InOutHistoryTimeline>> InOutValidateAsync(StringValues dbName, IdentityUser user,
        IPAddress remoteIpAddress,
        TimeKeepingValidationRequest request)
    {
        // Validate face from image
        var detectedUser = await _faceRecognitionService.DetectAndIdentifyFacesAsync(dbName ,request.File);
        if (detectedUser == null)
        {
            throw new Exception("Cannot detect user from image");
        }

        if (detectedUser.Username != user.UserName)
        {
            throw new Exception("The face from image not match with user face");
        }
        
        var ipAddress = GetIpClient(remoteIpAddress);
        var configuration = await GetConfigurationByIp(ipAddress);
        if (configuration == null)
        {
            throw new Exception($"TimeKeeping configuration with IP address {ipAddress} not found. Cannot check-in or check-out!");
        }
        var ipValid = ipAddress == configuration.IpAddress;

        // Validate location
        var clientLocation = new Location(request.Latitude, request.Longitude);
        var centerLocation = new Location(configuration.LatitudePoint, configuration.LongitudePoint);
        var distance = CalculateDistance(clientLocation, centerLocation);
        var locationValid = distance <= configuration.AllowedRadius;

        var logging = new TimeKeepingValidationResult
        {
            ClientLocation = clientLocation,
            ValidationLocation = centerLocation,
            Distance = distance,
            ClientIp = ipAddress,
            ValidationIp = configuration.IpAddress,
            IpValidation = ipValid,
            LocationValidation = locationValid,
            DeviceId = request.DeviceId,
            TargetId = configuration.Id
        };

        if (configuration.LocationValidationEnable &&!logging.LocationValidation)
        {
            throw new Exception("Location is invalid. Unable to continue timekeeping process!");
        }

        var history = await ProcessTimekeeping(user, logging, request);
        return history;
    }


    public string GetIpClient(IPAddress remoteIpAddress)
    {
        // Use the client IP address as needed
        string ipV4 = "";
        if (remoteIpAddress != null)
        {
            // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
            // This usually only happens when the browser is on the same machine as the server.
            if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
            {
                remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList
                    .First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            }

            ipV4 = remoteIpAddress.ToString();
        }

        return ipV4;
    }


    public async Task<List<InOutHistoryTimeline>> InOutQRValidateAsync( IdentityUser user,
        IPAddress remoteIpAddress,
        TimeKeepingQrValidationRequest request)
    {

        var configuration = await GetConfigurationByID(request.TagertId);
        if (configuration == null)
        {
            throw new Exception($"TimeKeeping configuration with TagertId: {request.TagertId} not found. Cannot check-in or check-out!");
        }

        // Validate location
        var clientLocation = new Location(request.Latitude, request.Longitude);
        var centerLocation = new Location(configuration.LatitudePoint, configuration.LongitudePoint);
        var distance = CalculateDistance(clientLocation, centerLocation);
        var locationValid = distance <= configuration.AllowedRadius;

        var logging = new TimeKeepingValidationResult
        {
            ClientLocation = clientLocation,
            ValidationLocation = centerLocation,
            Distance = distance,
            ClientIp = remoteIpAddress.ToString(),
            ValidationIp = configuration.IpAddress,
            IpValidation = true,
            LocationValidation = locationValid,
            DeviceId = request.DeviceId,
            TargetId = configuration.Id
        };

        if (configuration.LocationValidationEnable && !logging.LocationValidation)
        {
            throw new Exception("Location is invalid. Unable to continue timekeeping process!");
        }

        var history = await NewProcessTimekeeping(user, logging, request);
        return history;
    }
    private async Task<List<InOutHistoryTimeline>> NewProcessTimekeeping(
    IdentityUser user,
    TimeKeepingValidationResult logging,
    TimeKeepingQrValidationRequest request)
    {
        // Kiểm tra vị trí quét QR có hợp lệ không
        if (!IsValidLocation(request.Latitude, request.Longitude, logging.TargetId))
        {
            throw new Exception("Bạn không ở đúng vị trí chấm công.");
        }

        // Lấy lịch sử chấm công của user trong ngày
        var inOutLog = await _context.TimeKeepingInOutLogging.FirstOrDefaultAsync(x => x.UserName == user.UserName && x.Date == DateTime.Now.Date);
        var histories = JsonConvert.DeserializeObject<List<TimeKeepingInOuHistory>>(inOutLog?.Data ?? string.Empty)?.OrderBy(x => x.From).ToList();

        var traceLogs = JsonConvert.DeserializeObject<List<InOutTrace>>(inOutLog?.Traces ?? string.Empty)?.ToList() ?? new List<InOutTrace>();

        traceLogs.Add(new InOutTrace()
        {
            Date = DateTime.Now,
            Data = JsonConvert.SerializeObject(logging)
        });

        // Nếu chưa có log thì khởi tạo
        if (inOutLog == null)
        {
            histories = await InitTimeFrameAsync(user.Id);
            inOutLog = new TimeKeepingInOutLogging()
            {
                Date = DateTime.Today,
                UserName = user.UserName,
                UserId = user.Id
            };
            _context.TimeKeepingInOutLogging.Add(inOutLog);
        }

        if (histories?.Count > 0)
        {
            var targetTimeFrame = GetTargetTimeFrame(histories, request.TimeFrameFrom, request.TimeFrameTo);

            if (targetTimeFrame == null)
            {
                throw new Exception("Không tìm thấy ca làm việc trong thời gian này.");
            }

            // Check-in
            var isCheckin = targetTimeFrame.CheckinTime == null;
            if (isCheckin)
            {
                targetTimeFrame.CheckinTime = DateTime.Now;
                targetTimeFrame.CheckinTargetId = logging.TargetId;
                targetTimeFrame.CheckinTrace = JsonConvert.SerializeObject(logging);
            }

            // Check-out
            var isCheckout = !isCheckin && targetTimeFrame.CheckoutTime == null;
            if (isCheckout)
            {
                targetTimeFrame.CheckoutTime = DateTime.Now;
                targetTimeFrame.CheckoutTargetId = logging.TargetId;
                targetTimeFrame.CheckoutTrace = JsonConvert.SerializeObject(logging);
            }

            await StoreInOutHistoryAsync(user.Id, targetTimeFrame);
        }

        // Lưu vào Database
        inOutLog.Data = JsonConvert.SerializeObject(histories);
        inOutLog.Traces = JsonConvert.SerializeObject(traceLogs);

        if (inOutLog.Id > 0)
        {
            _context.TimeKeepingInOutLogging.Update(inOutLog);
        }

        await _context.SaveChangesAsync();
        return ConvertHistoryToTimeline(histories);
    }


    private async Task<List<TimeKeepingInOuHistory>> InitTimeFrameAsync(int userId)
    {
        // Get timeframe from configuration
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
        if (user == null)
        {
            throw new Exception("User not found. Cannot get time frame working");
        }

        var date = DateTime.Today;
        var shiftUser = await _context.ShiftUsers.FirstOrDefaultAsync(x => x.Month == date.Month && x.Year == date.Year);
        if (shiftUser is null)
        {
            throw new Exception("Shift for month not found");
        }

        var shiftUserDetail = await _context.ShiftUserDetails.FirstOrDefaultAsync(x => x.UserId == user.Id  && x.ShiftUserId == shiftUser.Id);
        if (shiftUserDetail is null)
        {
            throw new Exception("Not found shift user");
        }

        var dayProperty = shiftUserDetail.GetType().GetProperty($"Day{date.Day}");
        if (dayProperty == null)
        {
            throw new Exception($"Property Day{date.Day} not found in shiftUserDetail");
        }

        var dayValue = dayProperty.GetValue(shiftUserDetail, null);
        if (dayValue == null)
        {
            throw new Exception($"Value for property Day{date.Day} is null");
        }

        var symbolId = int.Parse(dayValue.ToString());


        var symbol = await _context.Symbols.FirstOrDefaultAsync(x => x.Id == symbolId);

        if (symbol == null)
        {
            throw new Exception("Shift's Symbol not found. Cannot get time frame working");
        }

        var timeFrames = await _context.Shifts
            .Where(x => x.SymbolId == symbol.Id)
            .Select(x => new TimeKeepingInOuHistory
            {
                SymbolId = x.SymbolId,
                From = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    DateTime.Today.Day,
                    x.TimeIn.Hours,
                    x.TimeIn.Minutes,
                    0),
                To = new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    DateTime.Today.Day,
                    x.TimeOut.Hours,
                    x.TimeOut.Minutes,
                    0),
                CheckInTimeThreshold = symbol.CheckInTimeThreshold,
                CheckOutTimeThreshold = symbol.CheckOutTimeThreshold
            }).ToListAsync();
        return timeFrames;
    }
    private bool IsValidLocation(double lat, double lon, int targetId)
    {
        var location = _context.Targets.FirstOrDefault(x => x.Id == targetId);
        if (location == null) return false;

        double distance = HaversineDistance(lat, lon, location.LatitudePoint, location.LongitudePoint);
        return distance <= location.AllowedRadius; 
    }
    private double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        double R = 6371e3; // Bán kính Trái Đất (m)
        double φ1 = lat1 * Math.PI / 180;
        double φ2 = lat2 * Math.PI / 180;
        double Δφ = (lat2 - lat1) * Math.PI / 180;
        double Δλ = (lon2 - lon1) * Math.PI / 180;

        double a = Math.Sin(Δφ / 2) * Math.Sin(Δφ / 2) +
                   Math.Cos(φ1) * Math.Cos(φ2) *
                   Math.Sin(Δλ / 2) * Math.Sin(Δλ / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        return R * c; // Khoảng cách (m)
    }
    private async Task<List<InOutHistoryTimeline>> ProcessTimekeeping(
        IdentityUser user,
        TimeKeepingValidationResult logging, 
        TimeKeepingValidationRequest request)
    {
        // Get in out log from db
        var inOutLog = await _context.TimeKeepingInOutLogging.FirstOrDefaultAsync(x => x.UserName == user.UserName && x.Date == DateTime.Now.Date);
        var histories = JsonConvert.DeserializeObject<List<TimeKeepingInOuHistory>>(inOutLog?.Data ?? string.Empty)?.OrderBy(x => x.From).ToList();

        var traceLogs = JsonConvert.DeserializeObject<List<InOutTrace>>(inOutLog?.Traces ?? string.Empty)?.ToList() 
                        ?? new List<InOutTrace>();
        
        traceLogs.Add(new InOutTrace()
        {
            Date = DateTime.Now,
            Data = JsonConvert.SerializeObject(logging)
        });
        
        // Add new case
        if (inOutLog == null)
        {
            histories = await InitTimeFrameAsync(user.Id);
            inOutLog = new TimeKeepingInOutLogging()
            {
                Date = DateTime.Today,
                UserName = user.UserName,
                UserId = user.Id
            };
            _context.TimeKeepingInOutLogging.Add(inOutLog);
        }

        if (histories?.Count > 0)
        {
            // Order history by timeline
            var targetTimeFrame = GetTargetTimeFrame(histories, request.TimeFrameFrom, request.TimeFrameTo);

            if (targetTimeFrame == null)
            {
                throw new Exception("No shifts found during this time frame");
            }
        
            // Check-in
            var isCheckin = targetTimeFrame.CheckinTime == null;
            if (isCheckin)
            {
                targetTimeFrame.CheckinTime = DateTime.Now;
                targetTimeFrame.CheckinTargetId = logging.TargetId;
                targetTimeFrame.CheckinTrace = JsonConvert.SerializeObject(logging);
            }
        
            // Check-out
            var isCheckout = !isCheckin && targetTimeFrame.CheckoutTime == null;
            if (isCheckout)
            {
                targetTimeFrame.CheckoutTime = DateTime.Now;
                targetTimeFrame.CheckoutTargetId = logging.TargetId;
                targetTimeFrame.CheckoutTrace = JsonConvert.SerializeObject(logging);
            }

            await StoreInOutHistoryAsync(
                userId: user.Id,
                timeFrame: targetTimeFrame
            );
        }
        
        // Store into Db
        inOutLog.Data = JsonConvert.SerializeObject(histories);
        inOutLog.Traces = JsonConvert.SerializeObject(traceLogs);

        if (inOutLog.Id > 0)
        {
            _context.TimeKeepingInOutLogging.Update(inOutLog);
        }
        
        await _context.SaveChangesAsync();
        return ConvertHistoryToTimeline(histories);
    }
    
    private double CalculateDistance(Location location1, Location location2)
    {
        const double earthRadiusKm = 6371;
        // Convert latitude and longitude from degrees to radians
        double lat1Rad = Math.PI * location1.Latitude / 180.0;
        double lon1Rad = Math.PI * location1.Longitude / 180.0;
        double lat2Rad = Math.PI * location2.Latitude / 180.0;
        double lon2Rad = Math.PI * location2.Longitude / 180.0;

        // Calculate differences between latitudes and longitudes
        double dLat = lat2Rad - lat1Rad;
        double dLon = lon2Rad - lon1Rad;

        // Haversine formula to calculate distance
        double a = Math.Pow(Math.Sin(dLat / 2), 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Pow(Math.Sin(dLon / 2), 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        double distance = earthRadiusKm * c;

        return distance;
    }

    public async Task<List<InOutHistoryTimeline>> GetInOutHistoryByDate(IdentityUser user, DateTime targetDate)
    {
        var inOutLogging =
            await _context.TimeKeepingInOutLogging.FirstOrDefaultAsync(x =>
                x.UserName == user.UserName && x.Date == targetDate.Date);

        var histories = inOutLogging != null
            ? JsonConvert.DeserializeObject<List<TimeKeepingInOuHistory>>(inOutLogging.Data ?? string.Empty)
            : await InitTimeFrameAsync(user.Id);
        return ConvertHistoryToTimeline(histories);
    }

    private List<InOutHistoryTimeline> ConvertHistoryToTimeline(List<TimeKeepingInOuHistory> histories)
    {
        histories = histories.OrderBy(x => x.From).ToList();
        List<InOutHistoryTimeline> result = new List<InOutHistoryTimeline>();
        foreach (var history in histories)
        {
            // IN
            result.Add(new InOutHistoryTimeline()
            {
                Type = "IN",
                Name = $"{history.From:HH:mm} - {history.To:HH:mm}",
                SubmitTime = history.CheckinTime,
                From = history.From,
                To = history.To,
                CheckInTimeThreshold = history.CheckInTimeThreshold,
                CheckOutTimeThreshold = history.CheckOutTimeThreshold
            });
            
            // OUT
            result.Add(new InOutHistoryTimeline()
            {
                Type = "OUT",
                Name = $"{history.From:HH:mm} - {history.To:HH:mm}",
                SubmitTime = history.CheckoutTime,
                From = history.From,
                To = history.To,
                IsCheckedIn = history.CheckinTime != null,
                CheckInTimeThreshold = history.CheckInTimeThreshold,
                CheckOutTimeThreshold = history.CheckOutTimeThreshold
            });
        }
        return result;
    }

    private async Task StoreInOutHistoryAsync(int userId,TimeKeepingInOuHistory timeFrame)
    {
        var history = await _context.InOutHistories.FirstOrDefaultAsync(x =>
            x.UserId == userId &&
            x.SymbolId == timeFrame.SymbolId &&
            x.TimeFrameFrom >= timeFrame.From &&
            x.TimeFrameTo <= timeFrame.To);

        if (history == null)
        {
            history = new InOutHistory()
            {
                SymbolId = timeFrame.SymbolId,
                UserId = userId,
                TimeFrameFrom = timeFrame.From,
                TimeFrameTo = timeFrame.To
            };
        }
        
        history.TimeIn = timeFrame.CheckinTime;
        history.TimeOut = timeFrame.CheckoutTime;
        history.TargetId = timeFrame.CheckinTargetId ?? timeFrame.CheckoutTargetId.GetValueOrDefault();
        history.TargetDate = DateTime.Now.Date;
        if (history.Id == 0)
        {
            var date = new DateTime(history.TimeIn.Value.Year, history.TimeIn.Value.Month, history.TimeIn.Value.Day);
            var timeType = history.TimeIn.Value.Hour < 12 ? "morning" : "afternoon";
            var meal = await _context.NumberOfMeals.FirstOrDefaultAsync(x => x.Date == date && x.TimeType == timeType);
            if(meal is null)
            {
                meal = new NumberOfMeal
                {
                    Date = date,
                    TimeType = timeType,
                };
            }
            meal.QuantityFromInOut += 1;
            _context.NumberOfMeals.Update(meal);
        }

        _ = history.Id > 0
            ? _context.InOutHistories.Update(history)
            : _context.InOutHistories.Add(history);

       
        await _context.SaveChangesAsync();
    }

    private Task<Target> GetConfigurationByIp(string iPAddress)
    {
        return _context.Targets.FirstOrDefaultAsync(x => x.IpAddress == iPAddress);
    }
    private Task<Target> GetConfigurationByID(int TagertId)
    {
        return _context.Targets.FirstOrDefaultAsync(x => x.Id == TagertId);
    }
    private TimeKeepingInOuHistory GetTargetTimeFrame(List<TimeKeepingInOuHistory> histories, DateTime timeFrameFrom, DateTime timeFrameTo)
    {
        var targetTimeFrame =
            histories.FirstOrDefault(x =>
                x.From == timeFrameFrom &&
                x.To == timeFrameTo
            );

        if (targetTimeFrame == null)
        {
            targetTimeFrame = histories.LastOrDefault(x => x.From.AddMinutes(-x.CheckInTimeThreshold) <= DateTime.Now);
        }

        return targetTimeFrame;
    }
}