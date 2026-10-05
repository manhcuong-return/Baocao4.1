/**************************************************
Mã sinh viên: 202418860
Họ tên: Lã Mạnh Cường
**************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace PayrollSystem
{
    // A.2 Lớp Employee cơ sở
    public abstract class Employee
    {
        private string employeeId;
        private string fullName;
        private string department;
        private decimal monthlyBonus;

        public string EmployeeId
        {
            get => employeeId;
            set => employeeId = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Mã nhân sự không rỗng.") : value;
        }

        public string FullName
        {
            get => fullName;
            set => fullName = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Họ tên không rỗng.") : value;
        }

        public string Department
        {
            get => department;
            set => department = string.IsNullOrWhiteSpace(value) ? throw new ArgumentException("Phòng ban không rỗng.") : value;
        }

        public decimal MonthlyBonus
        {
            get => monthlyBonus;
            protected set => monthlyBonus = value < 0 ? throw new ArgumentException("Thưởng không được âm.") : value;
        }

        // A.4 Nạp chồng constructor
        public Employee(string employeeId, string fullName)
        {
            EmployeeId = employeeId;
            FullName = fullName;
            Department = "Unassigned";
            MonthlyBonus = 0;
        }

        public Employee(string employeeId, string fullName, string department)
            : this(employeeId, fullName)
        {
            Department = department;
        }

        // A.5 Nạp chồng phương thức thưởng
        public void AddBonus(decimal amount)
        {
            if (amount <= 0) throw new ArgumentException("Khoản thưởng phải lớn hơn 0.");
            MonthlyBonus += amount;
        }

        public void AddBonus(decimal amount, string reason)
        {
            if (amount <= 0) throw new ArgumentException("Khoản thưởng phải lớn hơn 0.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Lý do không được rỗng.");
            MonthlyBonus += amount;
        }

        public void AddBonus(double rate, decimal referenceAmount, string reason)
        {
            if (rate <= 0 || rate > 0.5) throw new ArgumentException("Tỷ lệ thưởng phải lớn hơn 0 và không quá 0.5.");
            if (referenceAmount <= 0) throw new ArgumentException("Giá trị tham chiếu phải lớn hơn 0.");
            if (string.IsNullOrWhiteSpace(reason)) throw new ArgumentException("Lý do không được rỗng.");
            MonthlyBonus += (decimal)rate * referenceAmount;
        }

        // A.6 Ghi đè phương thức tính lương
        public abstract decimal CalculateGrossPay();

        public virtual string GetEmployeeType()
        {
            return "Employee";
        }

        public virtual void DisplayPayrollInfo()
        {
            Console.WriteLine($"Mã: {EmployeeId} | Tên: {FullName} | Phòng: {Department} | Thưởng: {MonthlyBonus:N0} | Loại: {GetEmployeeType()}");
        }
    }

    // A.3 Các lớp dẫn xuất: SalariedEmployee
    public class SalariedEmployee : Employee
    {
        private decimal monthlySalary;
        private decimal responsibilityAllowance;

        public decimal MonthlySalary
        {
            get => monthlySalary;
            set => monthlySalary = value < 0 ? throw new ArgumentException("Lương cố định không được âm.") : value;
        }

        public decimal ResponsibilityAllowance
        {
            get => responsibilityAllowance;
            set => responsibilityAllowance = value < 0 ? throw new ArgumentException("Phụ cấp trách nhiệm không được âm.") : value;
        }

        public SalariedEmployee(string employeeId, string fullName, decimal monthlySalary)
            : base(employeeId, fullName)
        {
            MonthlySalary = monthlySalary;
            ResponsibilityAllowance = 0;
        }

        public SalariedEmployee(string employeeId, string fullName, string department, decimal monthlySalary, decimal responsibilityAllowance)
            : base(employeeId, fullName, department)
        {
            MonthlySalary = monthlySalary;
            ResponsibilityAllowance = responsibilityAllowance;
        }

        public override decimal CalculateGrossPay()
        {
            return MonthlySalary + ResponsibilityAllowance + MonthlyBonus;
        }

        public override string GetEmployeeType() => "Salaried Employee";

        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"   => Lương cứng: {MonthlySalary:N0} | Phụ cấp: {ResponsibilityAllowance:N0} | Tổng thu nhập: {CalculateGrossPay():N0}");
        }
    }

    // A.3 Các lớp dẫn xuất: HourlyEmployee
    public class HourlyEmployee : Employee
    {
        private decimal hourlyRate;
        private double workedHours;

        public decimal HourlyRate
        {
            get => hourlyRate;
            set => hourlyRate = value < 0 ? throw new ArgumentException("Đơn giá giờ không được âm.") : value;
        }

        public double WorkedHours
        {
            get => workedHours;
            set
            {
                if (value < 0 || value > 250) throw new ArgumentException("Số giờ làm hợp lệ từ 0 đến 250.");
                workedHours = value;
            }
        }

        public HourlyEmployee(string employeeId, string fullName, decimal hourlyRate)
            : base(employeeId, fullName)
        {
            HourlyRate = hourlyRate;
            WorkedHours = 0;
        }

        public HourlyEmployee(string employeeId, string fullName, string department, decimal hourlyRate, double workedHours)
            : base(employeeId, fullName, department)
        {
            HourlyRate = hourlyRate;
            WorkedHours = workedHours;
        }

        public override decimal CalculateGrossPay()
        {
            decimal basePay;
            if (WorkedHours <= 160)
            {
                basePay = (decimal)WorkedHours * HourlyRate;
            }
            else
            {
                basePay = 160 * HourlyRate + (decimal)(WorkedHours - 160) * HourlyRate * 1.5m;
            }
            return basePay + MonthlyBonus;
        }

        public override string GetEmployeeType() => "Hourly Employee";

        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"   => Đơn giá: {HourlyRate:N0} | Số giờ: {WorkedHours} | Tổng thu nhập: {CalculateGrossPay():N0}");
        }
    }

    // A.3 Các lớp dẫn xuất: SalesEmployee
    public class SalesEmployee : Employee
    {
        private decimal baseSalary;
        private decimal salesRevenue;
        private double commissionRate;

        public decimal BaseSalary
        {
            get => baseSalary;
            set => baseSalary = value < 0 ? throw new ArgumentException("Lương cơ bản không được âm.") : value;
        }

        public decimal SalesRevenue
        {
            get => salesRevenue;
            set => salesRevenue = value < 0 ? throw new ArgumentException("Doanh số không được âm.") : value;
        }

        public double CommissionRate
        {
            get => commissionRate;
            set
            {
                if (value < 0 || value > 0.3) throw new ArgumentException("Tỷ lệ hoa hồng nằm trong khoảng từ 0 đến 0.3.");
                commissionRate = value;
            }
        }

        public SalesEmployee(string employeeId, string fullName, decimal baseSalary)
            : base(employeeId, fullName)
        {
            BaseSalary = baseSalary;
            SalesRevenue = 0;
            CommissionRate = 0;
        }

        public SalesEmployee(string employeeId, string fullName, string department, decimal baseSalary, decimal salesRevenue, double commissionRate)
            : base(employeeId, fullName, department)
        {
            BaseSalary = baseSalary;
            SalesRevenue = salesRevenue;
            CommissionRate = commissionRate;
        }

        public override decimal CalculateGrossPay()
        {
            return BaseSalary + SalesRevenue * (decimal)CommissionRate + MonthlyBonus;
        }

        public override string GetEmployeeType() => "Sales Employee";

        public override void DisplayPayrollInfo()
        {
            base.DisplayPayrollInfo();
            Console.WriteLine($"   => Lương cơ bản: {BaseSalary:N0} | Doanh số: {SalesRevenue:N0} | Hoa hồng: {CommissionRate:P} | Tổng thu nhập: {CalculateGrossPay():N0}");
        }
    }

    // A.7 Lớp Payroll
    public class Payroll
    {
        public string Period { get; set; }
        private List<Employee> employees;

        public Payroll(string period)
        {
            Period = period;
            employees = new List<Employee>();
        }

        public void AddEmployee(Employee employee)
        {
            if (employees.Any(e => e.EmployeeId == employee.EmployeeId))
            {
                throw new ArgumentException($"Không thể thêm: Nhân sự với mã {employee.EmployeeId} đã tồn tại trong bảng lương.");
            }
            employees.Add(employee);
        }

        public Employee FindEmployee(string employeeId)
        {
            return employees.FirstOrDefault(e => e.EmployeeId == employeeId);
        }

        public decimal CalculateTotalPayroll()
        {
            return employees.Sum(e => e.CalculateGrossPay());
        }

        public decimal CalculatePayrollByDepartment(string department)
        {
            return employees.Where(e => e.Department.Equals(department, StringComparison.OrdinalIgnoreCase))
                            .Sum(e => e.CalculateGrossPay());
        }

        public Employee FindHighestPaidEmployee()
        {
            if (!employees.Any()) return null;
            return employees.OrderByDescending(e => e.CalculateGrossPay()).First();
        }

        public void DisplayPayroll()
        {
            Console.WriteLine($"\n=== BẢNG LƯƠNG KỲ {Period} ===");
            if (!employees.Any())
            {
                Console.WriteLine("Danh sách nhân sự trống.");
                return;
            }
            foreach (var emp in employees)
            {
                emp.DisplayPayrollInfo();
            }
            Console.WriteLine("=================================");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Di chuyển cấu hình Encoding vào bên trong hàm Main
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            try
            {
                Payroll payroll = new Payroll("2026-09");

                // C: Dữ liệu kiểm thử
                // Nhân viên lương cố định
                SalariedEmployee e001 = new SalariedEmployee("E001", "Nguyễn Minh An", "Đào tạo", 15000000m, 2000000m);
                e001.AddBonus(1000000m); // Thêm thưởng cố định
                payroll.AddEmployee(e001);

                // Nhân viên theo giờ, không có giờ vượt ngưỡng
                HourlyEmployee e002 = new HourlyEmployee("E002", "Trần Thu Bình", "Hỗ trợ", 100000m, 150);
                e002.AddBonus(500000m); // Thêm thưởng cố định
                payroll.AddEmployee(e002);

                // Nhân viên theo giờ, có giờ vượt ngưỡng
                HourlyEmployee e003 = new HourlyEmployee("E003", "Lê Hoàng Chi", "Hỗ trợ", 100000m, 170);
                // Không có thưởng
                payroll.AddEmployee(e003);

                // Nhân viên kinh doanh
                SalesEmployee e004 = new SalesEmployee("E004", "Phạm Quốc Dũng", "Kinh doanh", 8000000m, 200000000m, 0.05);
                e004.AddBonus(0.02, 50000000m, "Thưởng đạt KPI mảng A"); // Thưởng theo tỷ lệ
                payroll.AddEmployee(e004);

                // Hiển thị toàn bộ bảng lương
                payroll.DisplayPayroll();

                // Kiểm tra kết quả tổng hợp
                Console.WriteLine($"\nTổng bảng lương mong đợi: 70,000,000 | Thực tế: {payroll.CalculateTotalPayroll():N0}");
                Console.WriteLine($"Tổng phòng Hỗ trợ mong đợi: 33,000,000 | Thực tế: {payroll.CalculatePayrollByDepartment("Hỗ trợ"):N0}");

                var highestPaid = payroll.FindHighestPaidEmployee();
                Console.WriteLine($"Nhân sự có thu nhập cao nhất: {highestPaid.FullName} ({highestPaid.CalculateGrossPay():N0})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
    }
}