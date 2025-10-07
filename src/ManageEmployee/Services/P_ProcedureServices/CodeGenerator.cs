using ManageEmployee.Dal.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace ManageEmployee.Services.P_ProcedureServices
{
    public class CodeGenerator
    {
        private readonly ApplicationDbContext _context;

        public CodeGenerator(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateProcedureCodeAsync()
        {
            int month = DateTime.Now.Month;
            int year = DateTime.Now.Year % 100;

            // Lấy giá trị lớn nhất hiện tại từ DB hoặc khởi tạo với 0
            int currentNumber = await _context.P_Leave
                .Where(p => p.CreatedAt.Month == month && p.CreatedAt.Year % 100 == year)
                .CountAsync();

            string code;
            bool isDuplicate;

            do
            {
                currentNumber++; // Tăng giá trị số thứ tự
                code = $"NP{month:D2}-{year:D2}-{currentNumber:D4}";

                // Kiểm tra trong DB xem mã đã tồn tại hay chưa
                isDuplicate = await _context.P_Leave.AnyAsync(p => p.ProcedureNumber == code);

            } while (isDuplicate); // Nếu trùng, lặp lại quá trình này

            return code; // Trả về mã không trùng lặp
        }
    }
}
