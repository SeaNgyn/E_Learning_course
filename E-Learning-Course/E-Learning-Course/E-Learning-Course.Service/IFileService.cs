using E_Learning_Course.Data.Entities;
using E_Learning_Course.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Learning_Course.Service
{
    public interface IFileService
    {
        Task<List<BlobDto>> ListAsync();
        Task<BlobResponseDto> UploadAsync(IFormFile blob);
        Task<Certificate> GenerateAndUploadCertificateAsync(int enrollmentId, string userName, string courseName);
    }
}
