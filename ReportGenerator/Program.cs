using System;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;

namespace SupermarketApp.ReportGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Thiết lập license context cho EPPlus
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            
            try
            {
                CreateReport();
                Console.WriteLine("✅ Đã tạo file báo cáo DOCX thành công!");
                Console.WriteLine("📁 File: BAO_CAO_SUPERMARKET_APP.docx");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Lỗi khi tạo file: {ex.Message}");
            }
            
            Console.WriteLine("Nhấn Enter để thoát...");
            Console.ReadLine();
        }
        
        static void CreateReport()
        {
            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add("Báo cáo");
                
                // Thiết lập font mặc định
                worksheet.Cells.Style.Font.Name = "Times New Roman";
                worksheet.Cells.Style.Font.Size = 12;
                
                int row = 1;
                
                // Tiêu đề chính
                worksheet.Cells[row, 1].Value = "BÁO CÁO ĐỒ ÁN MÔN HỌC";
                worksheet.Cells[row, 1].Style.Font.Size = 16;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(row).Height = 30;
                row++;
                
                worksheet.Cells[row, 1].Value = "ỨNG DỤNG QUẢN LÝ BÁN HÀNG SIÊU THỊ";
                worksheet.Cells[row, 1].Style.Font.Size = 14;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(row).Height = 25;
                row += 2;
                
                // Thông tin sinh viên
                worksheet.Cells[row, 1].Value = "Sinh viên thực hiện:";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;
                
                worksheet.Cells[row, 1].Value = "Hồ Viết Hiệp";
                row++;
                worksheet.Cells[row, 1].Value = "Trương Dương Bảo Minh";
                row++;
                worksheet.Cells[row, 1].Value = "Nguyễn Đình Khanh";
                row++;
                worksheet.Cells[row, 1].Value = "Phan Lâm Tuấn Kiệt";
                row += 2;
                
                worksheet.Cells[row, 1].Value = "Giảng viên hướng dẫn: ThS. Dương Thành Phết";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;
                worksheet.Cells[row, 1].Value = "Trường Đại học Công Nghệ Thành Phố Hồ Chí Minh";
                row++;
                worksheet.Cells[row, 1].Value = "Khoa Công Nghệ Thông Tin";
                row++;
                worksheet.Cells[row, 1].Value = "TP. Hồ Chí Minh, tháng 4 năm 2025";
                row += 3;
                
                // Lời cảm ơn
                worksheet.Cells[row, 1].Value = "LỜI CẢM ƠN";
                worksheet.Cells[row, 1].Style.Font.Size = 14;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                row += 2;
                
                string thanksText = @"Lời đầu tiên, cho nhóm chúng em xin gửi lời cảm ơn chân thành đến quý thầy, cô giảng viên Trường Đại học Công Nghệ Thành Phố Hồ Chí Minh và quý thầy cô khoa Công Nghệ Thông Tin đã giúp cho nhóm chúng em có những kiến thức cơ bản làm nền tảng để thực hiện đồ án này.

Đặc biệt, nhóm chúng em xin gửi lời cảm ơn và lòng biết ơn sâu sắc nhất tới Giảng viên – ThS. Dương Thành Phết, người đã hướng dẫn cho chúng em trong suốt thời gian làm đề tài. Thầy đã trực tiếp hướng dẫn tận tình, sửa chữa và đóng góp nhiều ý kiến quý báu giúp cho nhóm chúng em hoàn thành tốt báo cáo môn học của mình. Một lần nữa chúng em xin chân thành cảm ơn thầy và chúc thầy dồi dào sức khỏe.

Trong thời gian một học kỳ thực hiện đề tài này, nhóm chúng em đã vận dụng những kiến thức nền tảng đã tích lũy đồng thời kết hợp với việc học hỏi và nghiên cứu những kiến thức mới từ thầy cô, bạn bè cũng như nhiều nguồn tài liệu tham khảo. 

Từ đó, nhóm chúng em đã vận dụng tối đa những gì mà chúng em đã thu thập được để hoàn thành tốt đề tài mà chúng em đã chọn một cách tốt nhất. Tuy nhiên, trong quá trình tìm kiếm và thu thập những kiến thức đó thì một phần chuyên môn chúng em vẫn còn hạn chế và bản thân còn thiếu nhiều kinh nghiệm thực tiễn nên nội dung của báo cáo không tránh khỏi những thiếu sót, chúng em rất mong nhận được sự góp ý, chỉ bảo thêm của quý thầy cô nhằm hoàn thiện những kiến thức của mình để nhóm chúng em có thể dùng làm hành trang thực hiện tiếp các đề tài khác trong tương lai cũng như là trong việc học tập và làm việc sau này.";
                
                worksheet.Cells[row, 1].Value = thanksText;
                worksheet.Cells[row, 1].Style.WrapText = true;
                worksheet.Row(row).Height = 200;
                row += 10;
                
                // Lời cam đoan
                worksheet.Cells[row, 1].Value = "LỜI CAM ĐOAN";
                worksheet.Cells[row, 1].Style.Font.Size = 14;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                row += 2;
                
                string pledgeText = @"Nhóm chúng em xin cam đoan rằng đề tài ""Ứng dụng Quản lý Bán hàng Siêu thị"" được tiến hành một cách minh bạch và công khai. Toàn bộ nội dung và kết quả được sự cố gắng cũng như là sự nỗ lực của bản thân cùng với sự giúp đỡ không nhỏ từ thầy hướng dẫn.

Nhóm chúng em xin cam đoan toàn bộ nội dung và kết quả trong đồ án là trung thực và không sao chép hay sử dụng kết quả của một đề tài nào tương tự. Trong quá trình viết bài có sự tham khảo một số tài liệu và có nguồn gốc rõ ràng và được trích dẫn đầy đủ ở phần trích dẫn tham khảo.

Nhóm chúng em xin chịu toàn bộ trách nhiệm nếu có bất kỳ sự sao chép, gian dối kết quả nào trong sản phẩm đồ án này.";
                
                worksheet.Cells[row, 1].Value = pledgeText;
                worksheet.Cells[row, 1].Style.WrapText = true;
                worksheet.Row(row).Height = 150;
                row += 8;
                
                // Mục lục
                worksheet.Cells[row, 1].Value = "MỤC LỤC";
                worksheet.Cells[row, 1].Style.Font.Size = 14;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                row += 2;
                
                string[] tocItems = {
                    "CHƯƠNG 1: CƠ SỞ LÝ THUYẾT",
                    "1.1 Giới thiệu đề tài",
                    "1.1.1 Khảo sát hiện trạng",
                    "1.1.2 Ứng dụng với hiện trạng", 
                    "1.1.3 Yêu Cầu Phi Chức Năng",
                    "1.2 Cơ Sở Lý Thuyết",
                    "1.2.1 Công cụ thực hiện",
                    "1.2.2 Visual Studio",
                    "1.2.3 C#",
                    "1.2.4 SunnyUI",
                    "1.2.5 Entity Framework Core",
                    "CHƯƠNG 2: PHÂN TÍCH, THIẾT KẾ HỆ THỐNG VÀ DỮ LIỆU",
                    "2.1 Sơ đồ usecase tổng quan",
                    "2.2 Cơ sở dữ liệu",
                    "2.3 Kiến trúc hệ thống",
                    "CHƯƠNG 3: ỨNG DỤNG QUẢN LÝ BÁN HÀNG SIÊU THỊ",
                    "3.1 Các chức năng của ứng dụng",
                    "3.2 Cải tiến tương lai",
                    "CHƯƠNG 4: KẾT LUẬN VÀ HƯỚNG PHÁT TRIỂN",
                    "4.1 Kết luận",
                    "4.2 Hướng phát triển",
                    "TÀI LIỆU THAM KHẢO"
                };
                
                string[] pageNumbers = {
                    "1", "1", "1", "1", "2", "3", "3", "3", "4", "5", "6", 
                    "7", "7", "8", "13", "17", "17", "27", "29", "29", "30", "31"
                };
                
                for (int i = 0; i < tocItems.Length; i++)
                {
                    worksheet.Cells[row, 1].Value = tocItems[i];
                    worksheet.Cells[row, 2].Value = pageNumbers[i];
                    row++;
                }
                
                row += 2;
                
                // Chương 1
                worksheet.Cells[row, 1].Value = "CHƯƠNG 1: CƠ SỞ LÝ THUYẾT";
                worksheet.Cells[row, 1].Style.Font.Size = 14;
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                worksheet.Cells[row, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                row += 2;
                
                worksheet.Cells[row, 1].Value = "1.1 Giới thiệu đề tài";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;
                
                worksheet.Cells[row, 1].Value = "1.1.1 Khảo sát hiện trạng";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;
                
                string surveyText = @"Hiện nay, việc quản lý bán hàng tại các siêu thị và cửa hàng tạp hóa đang gặp nhiều thách thức trong việc theo dõi tồn kho, quản lý khách hàng, và tạo báo cáo kinh doanh. Các phương pháp quản lý truyền thống bằng sổ sách hoặc Excel đã không còn phù hợp với quy mô hoạt động ngày càng lớn và yêu cầu báo cáo chi tiết.

Các vấn đề chính mà các siêu thị đang gặp phải bao gồm:
- Khó khăn trong việc theo dõi tồn kho chính xác
- Quản lý thông tin khách hàng không hiệu quả  
- Tạo hóa đơn và báo cáo mất nhiều thời gian
- Thiếu hệ thống phân quyền cho nhân viên
- Khó khăn trong việc phân tích xu hướng bán hàng

Vì vậy, việc phát triển một ứng dụng quản lý bán hàng chuyên nghiệp với giao diện hiện đại và tính năng đầy đủ là rất cần thiết để nâng cao hiệu quả hoạt động kinh doanh.";
                
                worksheet.Cells[row, 1].Value = surveyText;
                worksheet.Cells[row, 1].Style.WrapText = true;
                worksheet.Row(row).Height = 120;
                row += 6;
                
                worksheet.Cells[row, 1].Value = "1.1.2 Ứng dụng với hiện trạng";
                worksheet.Cells[row, 1].Style.Font.Bold = true;
                row++;
                
                string applicationText = @"Ứng dụng SupermarketApp được thiết kế để giải quyết các vấn đề trên với những tính năng chính:

Hệ thống quản lý sản phẩm:
- Quản lý thông tin sản phẩm đầy đủ (tên, giá, tồn kho, barcode)
- Phân loại sản phẩm theo danh mục
- Tìm kiếm sản phẩm nhanh chóng
- Cảnh báo hết hàng tự động

Hệ thống quản lý khách hàng:
- Lưu trữ thông tin khách hàng chi tiết
- Hệ thống điểm tích lũy
- Lịch sử mua hàng
- Phân loại khách hàng (VIP, thường, vãng lai)

Hệ thống bán hàng:
- Tạo hóa đơn nhanh chóng và chính xác
- Tính tổng tiền tự động
- In hóa đơn trực tiếp
- Liên kết với khách hàng

Hệ thống quản lý nhân viên:
- Phân quyền Admin/Nhân viên
- Theo dõi hoạt động bán hàng
- Quản lý tài khoản an toàn

Hệ thống báo cáo:
- Báo cáo doanh thu theo thời gian
- Thống kê sản phẩm bán chạy
- Báo cáo tồn kho
- Phân tích xu hướng kinh doanh

Hệ thống nhập hàng:
- Quản lý nhà cung cấp
- Tạo phiếu nhập hàng
- Cập nhật tồn kho tự động
- Theo dõi chi phí nhập hàng";
                
                worksheet.Cells[row, 1].Value = applicationText;
                worksheet.Cells[row, 1].Style.WrapText = true;
                worksheet.Row(row).Height = 200;
                row += 10;
                
                // Thiết lập độ rộng cột
                worksheet.Column(1).Width = 80;
                worksheet.Column(2).Width = 10;
                
                // Lưu file
                var fileInfo = new FileInfo("BAO_CAO_SUPERMARKET_APP.xlsx");
                package.SaveAs(fileInfo);
            }
        }
    }
}



