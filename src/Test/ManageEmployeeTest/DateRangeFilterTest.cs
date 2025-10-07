using ManageEmployee.DataTransferObject.InOutModels;
using NUnit.Framework;
using Shouldly;

namespace ManageEmployeeTest;

public class DateRangeFilterTest
{
    [Test]
    public void DateRangeFilter_GetMonthsAndYears_FromDate_IsNull()
    {
        var dateRangeFilter = new DateRangeFilter();
        var result = dateRangeFilter.GetMonthsAndYears().ToList();

        var currentTime = DateTime.Now;
        result.ShouldBeEquivalentTo(new List<(int Month, int Year)>
        {
            (currentTime.Month, currentTime.Year)    
        });
    }

    [Test]
    public void DateRangeFilter_GetMonthsAndYears_ToDate_IsNull()
    {
        var fromDate = DateTime.Now;
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate
        };

        var result = dateRangeFilter.GetMonthsAndYears().ToList();
        result.ShouldBeEquivalentTo(new List<(int Month, int Year)>
        {
            (fromDate.Month, fromDate.Year)
        });
    }

    [Test]
    public void DateRangeFilter_GetMonthsAndYears_DateRange_TheSameYear()
    {
        var fromDate = new DateTime(2024, 01,01);
        var toDate = new DateTime(2024, 06, 01);
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = dateRangeFilter.GetMonthsAndYears().ToList();
        result.ShouldBeEquivalentTo(new List<(int Month, int Year)>
        {
            (1, 2024),
            (2, 2024),
            (3, 2024),
            (4, 2024),
            (5, 2024),
            (6, 2024),
        });
    }

    [Test]
    public void DateRangeFilter_GetMonthsAndYears_DateRange_TheDifferentYear()
    {
        var fromDate = new DateTime(2024, 08, 01);
        var toDate = new DateTime(2025, 06, 01);
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = dateRangeFilter.GetMonthsAndYears().ToList();
        result.ShouldBeEquivalentTo(new List<(int Month, int Year)>
        {
            (8, 2024),
            (9, 2024),
            (10, 2024),
            (11, 2024),
            (12, 2024),
            (1, 2025),
            (2, 2025),
            (3, 2025),
            (4, 2025),
            (5, 2025),
            (6, 2025),
        });
    }

    [Test]
    public void DateRangeFilter_GetDates_FromDate_IsNull()
    {
        var dateRangeFilter = new DateRangeFilter();
        var result = dateRangeFilter.GetDates().ToList();

        var currentTime = DateTime.Now;
        result.ShouldBeEquivalentTo(new List<(int Day, int Month, int Year)>
        {
            (currentTime.Day, currentTime.Month, currentTime.Year)
        });
    }

    [Test]
    public void DateRangeFilter_GetDates_ToDate_IsNull()
    {
        var fromDate = DateTime.Now;
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate
        };

        var result = dateRangeFilter.GetDates().ToList();
        result.ShouldBeEquivalentTo(new List<(int Day, int Month, int Year)>
        {
            (fromDate.Day, fromDate.Month, fromDate.Year)
        });
    }

    [Test]
    public void DateRangeFilter_GetDates_DateRange_TheSameYear()
    {
        var fromDate = new DateTime(2024, 03, 05);
        var toDate = new DateTime(2024, 04, 15);
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = dateRangeFilter.GetDates().ToList();
        result.ShouldBeEquivalentTo(new List<(int Day, int Month, int Year)>
        {
            (05, 3, 2024),
            (06, 3, 2024),
            (07, 3, 2024),
            (08, 3, 2024),
            (09, 3, 2024),
            (10, 3, 2024),
            (11, 3, 2024),
            (12, 3, 2024),
            (13, 3, 2024),
            (14, 3, 2024),
            (15, 3, 2024),
            (16, 3, 2024),
            (17, 3, 2024),
            (18, 3, 2024),
            (19, 3, 2024),
            (20, 3, 2024),
            (21, 3, 2024),
            (22, 3, 2024),
            (23, 3, 2024),
            (24, 3, 2024),
            (25, 3, 2024),
            (26, 3, 2024),
            (27, 3, 2024),
            (28, 3, 2024),
            (29, 3, 2024),
            (30, 3, 2024),
            (31, 3, 2024),
            (1, 4, 2024),
            (2, 4, 2024),
            (3, 4, 2024),
            (4, 4, 2024),
            (5, 4, 2024),
            (6, 4, 2024),
            (7, 4, 2024),
            (8, 4, 2024),
            (9, 4, 2024),
            (10, 4, 2024),
            (11, 4, 2024),
            (12, 4, 2024),
            (13, 4, 2024),
            (14, 4, 2024),
            (15, 4, 2024),
        });
    }

    [Test]
    public void DateRangeFilter_GetDates_DateRange_TheDifferentYear()
    {
        var fromDate = new DateTime(2024, 12, 05);
        var toDate = new DateTime(2025, 01, 15);
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate,
            ToDate = toDate
        };

        var result = dateRangeFilter.GetDates().ToList();
        result.ShouldBeEquivalentTo(new List<(int Day, int Month, int Year)>
        {
            (05, 12, 2024),
            (06, 12, 2024),
            (07, 12, 2024),
            (08, 12, 2024),
            (09, 12, 2024),
            (10, 12, 2024),
            (11, 12, 2024),
            (12, 12, 2024),
            (13, 12, 2024),
            (14, 12, 2024),
            (15, 12, 2024),
            (16, 12, 2024),
            (17, 12, 2024),
            (18, 12, 2024),
            (19, 12, 2024),
            (20, 12, 2024),
            (21, 12, 2024),
            (22, 12, 2024),
            (23, 12, 2024),
            (24, 12, 2024),
            (25, 12, 2024),
            (26, 12, 2024),
            (27, 12, 2024),
            (28, 12, 2024),
            (29, 12, 2024),
            (30, 12, 2024),
            (31, 12, 2024),
            (1, 01, 2025),
            (2, 01, 2025),
            (3, 01, 2025),
            (4, 01, 2025),
            (5, 01, 2025),
            (6, 01, 2025),
            (7, 01, 2025),
            (8, 01, 2025),
            (9, 01, 2025),
            (10, 01, 2025),
            (11, 01, 2025),
            (12, 01, 2025),
            (13, 01, 2025),
            (14, 01, 2025),
            (15, 01, 2025),
        });
    }

    [Test]
    public void DateRangeFilter_IsBetween_FromDate_IsNull()
    {
        var fromDate = DateTime.Now;
        var dateRangeFilter = new DateRangeFilter();

        var result = dateRangeFilter.IsBetween(fromDate);
        result.ShouldBe(true);
    }

    [Test]
    public void DateRangeFilter_IsBetween_ToDate_IsNull()
    {
        var fromDate = DateTime.Now;
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate,
        };

        var result = dateRangeFilter.IsBetween(fromDate);
        result.ShouldBe(true);
    }

    [Test]
    public void DateRangeFilter_IsBetween()
    {
        var fromDate = DateTime.Now;
        var dateRangeFilter = new DateRangeFilter
        {
            FromDate = fromDate,
            ToDate = fromDate.AddMonths(2)
        };

        var result = dateRangeFilter.IsBetween(fromDate);
        result.ShouldBe(true);
    }
}