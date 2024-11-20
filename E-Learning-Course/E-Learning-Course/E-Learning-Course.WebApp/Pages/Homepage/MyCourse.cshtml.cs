using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using E_Learning_Course.Data.Entities;
using E_Learning_Course.Service;
using E_Learning_Course.ViewModels;
using System.Security.Claims;
using E_Learning_Course.Data;

namespace E_Learning_Course.WebApp.Pages.Homepage
{
    public class MyCourseModel : PageModel
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IFileService _fileService;
        private readonly RepositoryContext _context;
        public MyCourseModel(IEnrollmentService enrollmentService, IFileService fileService, RepositoryContext context)
        {
            _enrollmentService = enrollmentService;
            _fileService = fileService;
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortField { get; set; } = "name";

        [BindProperty(SupportsGet = true)]
        public bool IsAscending { get; set; } = true;

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        public PaginatedResult<Enrollment> Enrollments { get; set; }
        public Dictionary<int, string> CertificateUrls { get; set; } = new Dictionary<int, string>();
        public async Task OnGetAsync()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            Enrollments = await _enrollmentService.GetEnrollmentsAsync(SearchTerm, SortField, IsAscending, Page, PageSize, userIdClaim.Value);
            foreach (var enrollment in Enrollments.Items)
            {
                if (enrollment.Progress == 100)
                {
                    var certificate = _context.Certificates
                        .Where(c => c.EnrollmentId == enrollment.Id)
                        .Select(c => c.CertificateUrl) // Assuming the URL is stored in the `Url` field
                        .FirstOrDefault();

                    if (certificate == null)
                    {
                        // Generate and upload if the certificate doesn't exist
                        Certificate certi = await _fileService.GenerateAndUploadCertificateAsync(enrollment.Id, enrollment.User.UserName, enrollment.Course.Title);
                        CertificateUrls[enrollment.Id] = certi.CertificateUrl;
                    }
                    else
                    {
                        CertificateUrls[enrollment.Id] = certificate;
                    }
                }
            }
        }
    }
}
