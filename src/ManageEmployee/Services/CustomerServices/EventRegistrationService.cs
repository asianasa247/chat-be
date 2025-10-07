using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManageEmployee.DataTransferObject.EventModels;
using ManageEmployee.Dal.DbContexts;
using ManageEmployee.Entities.CustomerEntities;
using Microsoft.EntityFrameworkCore;
using ManageEmployee.Services.Interfaces.Customers;

namespace ManageEmployee.Services.CustomerServices
{
    public class EventRegistrationService : IEventRegistrationService
    {
        private readonly ApplicationDbContext _db;

        public EventRegistrationService(ApplicationDbContext db)
        {
            _db = db;
        }

        // NEW: check phone tồn tại không
        public async Task<(bool Exists, int? CustomerId)> CheckPhoneAsync(string phone)
        {
            var found = await _db.Customers
                .AsNoTracking()
                .Where(c => c.Phone == phone)
                .Select(c => new { c.Id })
                .FirstOrDefaultAsync();

            return (found != null, found?.Id);
        }

        // ----------------- POST: Register -----------------
        public async Task<EventRegistrationResponse> RegisterAsync(EventRegistrationRequest dto)
        {
            if (dto.EventId <= 0) throw new ArgumentException("eventId is required");
            if (string.IsNullOrWhiteSpace(dto.FullName)) throw new ArgumentException("fullname is required");
            if (string.IsNullOrWhiteSpace(dto.Phone)) throw new ArgumentException("phone is required");

            var ev = await _db.EventCustomers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == dto.EventId);
            if (ev == null) throw new InvalidOperationException("Event not found");

            await using var tx = await _db.Database.BeginTransactionAsync();

            // Upsert Customer theo Phone
            var customer = await _db.Customers.FirstOrDefaultAsync(c => c.Phone == dto.Phone);
            var isNew = false;
            int? taxId = null;

            if (customer == null)
            {
                // Tạo mới customer
                customer = new Customer
                {
                    Name = dto.FullName,
                    Phone = dto.Phone,
                    Email = dto.Email ?? string.Empty,
                    ProvinceId = dto.ProvinceId,
                    DistrictId = dto.DistrictId,
                    Password = dto.Phone, // rule hiện tại
                    EventCustomerId = dto.EventId,
                    CreateAt = DateTime.Now
                };
                _db.Customers.Add(customer);
                await _db.SaveChangesAsync();
                isNew = true;

                // Tạo mới TaxInfo nếu FE truyền
                var needCreateTax =
                       !string.IsNullOrWhiteSpace(dto.CompanyName)
                    || !string.IsNullOrWhiteSpace(dto.Address)
                    || !string.IsNullOrWhiteSpace(dto.TaxCode)
                    || !string.IsNullOrWhiteSpace(dto.Phone);

                if (needCreateTax)
                {
                    var taxNew = new CustomerTaxInformation
                    {
                        CustomerId = customer.Id,
                        CompanyName = dto.CompanyName,
                        Address = dto.Address,
                        TaxCode = dto.TaxCode,
                        Phone = dto.Phone
                    };
                    _db.CustomerTaxInformations.Add(taxNew);
                    await _db.SaveChangesAsync();
                    taxId = taxNew.Id;
                }
            }
            else
            {
                // ĐÃ TỒN TẠI: CHỈ cập nhật EventCustomerId theo yêu cầu
                if (customer.EventCustomerId != dto.EventId)
                {
                    customer.EventCustomerId = dto.EventId;
                    _db.Customers.Update(customer);
                    await _db.SaveChangesAsync();
                }

                // Không cập nhật các trường còn lại; không upsert TaxInfo
                var tax = await _db.CustomerTaxInformations
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(t => t.CustomerId == customer.Id);
                taxId = tax?.Id;
            }

            await tx.CommitAsync();

            return new EventRegistrationResponse
            {
                CustomerId = customer.Id,
                CustomerTaxInformationId = taxId,
                EventCustomerId = dto.EventId,
                IsNewCustomer = isNew
            };
        }

        // ----------------- GET: All -----------------
        public async Task<(IEnumerable<EventRegistrationListItem> Items, int Total)>
            GetAllAsync(int pageIndex, int pageSize)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 20;

            var baseQuery =
                from c in _db.Customers.AsNoTracking()
                where c.EventCustomerId != null
                join e in _db.EventCustomers.AsNoTracking()
                    on c.EventCustomerId equals e.Id
                join t in _db.CustomerTaxInformations.AsNoTracking()
                    on c.Id equals t.CustomerId into jt
                from t in jt.DefaultIfEmpty()
                select new EventRegistrationListItem
                {
                    CustomerId = c.Id,
                    CustomerCreatedAt = c.CreateAt,
                    EventCustomerId = e.Id,
                    EventName = e.EventName,
                    EventCode = e.EventCode,
                    EventStartDate = e.StartDate,
                    EventEndDate = e.EndDate,
                    FullName = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    ProvinceId = c.ProvinceId,
                    DistrictId = c.DistrictId,
                    CompanyName = t != null ? t.CompanyName : null,
                    TaxCode = t != null ? t.TaxCode : null,
                    TaxPhone = t != null ? t.Phone : null,
                    TaxAddress = t != null ? t.Address : null
                };

            var total = await baseQuery.CountAsync();
            var items = await baseQuery
                .OrderByDescending(x => x.CustomerCreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        // ----------------- GET: By Event -----------------
        public async Task<(IEnumerable<EventRegistrationListItem> Items, int Total)>
            GetByEventAsync(int eventId, int pageIndex, int pageSize)
        {
            if (pageIndex <= 0) pageIndex = 1;
            if (pageSize <= 0) pageSize = 20;

            var baseQuery =
                from c in _db.Customers.AsNoTracking()
                where c.EventCustomerId == eventId
                join e in _db.EventCustomers.AsNoTracking()
                    on c.EventCustomerId equals e.Id
                join t in _db.CustomerTaxInformations.AsNoTracking()
                    on c.Id equals t.CustomerId into jt
                from t in jt.DefaultIfEmpty()
                select new EventRegistrationListItem
                {
                    CustomerId = c.Id,
                    CustomerCreatedAt = c.CreateAt,
                    EventCustomerId = e.Id,
                    EventName = e.EventName,
                    EventCode = e.EventCode,
                    EventStartDate = e.StartDate,
                    EventEndDate = e.EndDate,
                    FullName = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    ProvinceId = c.ProvinceId,
                    DistrictId = c.DistrictId,
                    CompanyName = t != null ? t.CompanyName : null,
                    TaxCode = t != null ? t.TaxCode : null,
                    TaxPhone = t != null ? t.Phone : null,
                    TaxAddress = t != null ? t.Address : null
                };

            var total = await baseQuery.CountAsync();
            var items = await baseQuery
                .OrderByDescending(x => x.CustomerCreatedAt)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, total);
        }

        // ----------------- GET: By Customer -----------------
        public async Task<EventRegistrationDetail?> GetByCustomerAsync(int customerId)
        {
            var query =
                from c in _db.Customers.AsNoTracking()
                where c.Id == customerId
                join e in _db.EventCustomers.AsNoTracking()
                    on c.EventCustomerId equals e.Id into je
                from e in je.DefaultIfEmpty()
                join t in _db.CustomerTaxInformations.AsNoTracking()
                    on c.Id equals t.CustomerId into jt
                from t in jt.DefaultIfEmpty()
                select new EventRegistrationDetail
                {
                    CustomerId = c.Id,
                    CustomerCreatedAt = c.CreateAt,
                    EventCustomerId = e != null ? e.Id : 0,
                    EventName = e != null ? e.EventName : string.Empty,
                    EventCode = e != null ? e.EventCode : null,
                    EventStartDate = e != null ? e.StartDate : DateTime.MinValue,
                    EventEndDate = e != null ? e.EndDate : null,
                    FullName = c.Name,
                    Email = c.Email,
                    Phone = c.Phone,
                    ProvinceId = c.ProvinceId,
                    DistrictId = c.DistrictId,
                    CompanyName = t != null ? t.CompanyName : null,
                    TaxCode = t != null ? t.TaxCode : null,
                    TaxPhone = t != null ? t.Phone : null,
                    TaxAddress = t != null ? t.Address : null
                };

            var data = await query.FirstOrDefaultAsync();

            if (data == null || data.EventCustomerId == 0) return null;

            return data;
        }
    }
}
