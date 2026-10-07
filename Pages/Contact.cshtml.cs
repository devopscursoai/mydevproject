using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Myproject.Pages
{
    public class ContactModel : PageModel
    {
        private readonly ILogger<ContactModel> _logger;
        private readonly IConfiguration _configuration;

        public ContactModel(ILogger<ContactModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        [BindProperty]
        public ContactInput Input { get; set; }

        public void OnGet()
        {
        }

        public class ContactInput
        {
            [Required]
            [Display(Name = "Your name")]
            public string Name { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Your email")]
            public string Email { get; set; }

            [Required]
            public string Subject { get; set; }

            [Required]
            [Display(Name = "Message")]
            public string Message { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Try to read SMTP config. If not present, persist to local folder.
            var smtpSection = _configuration.GetSection("Smtp");
            var host = smtpSection["Host"];

            try
            {
                if (!string.IsNullOrEmpty(host))
                {
                    var port = int.TryParse(smtpSection["Port"], out var p) ? p : 25;
                    var enableSsl = bool.TryParse(smtpSection["EnableSsl"], out var s) && s;
                    var from = smtpSection["From"] ?? Input.Email;
                    var to = smtpSection["To"] ?? from;

                    var mail = new MailMessage(from, to)
                    {
                        Subject = Input.Subject,
                        Body = $"Name: {Input.Name}\nEmail: {Input.Email}\n\n{Input.Message}",
                    };

                    using var client = new SmtpClient(host, port)
                    {
                        EnableSsl = enableSsl
                    };

                    var username = smtpSection["Username"];
                    var password = smtpSection["Password"];
                    if (!string.IsNullOrEmpty(username))
                    {
                        client.Credentials = new NetworkCredential(username, password);
                    }

                    await client.SendMailAsync(mail);
                    TempData["Success"] = "Your message was sent. Thank you!";
                    _logger.LogInformation("Contact message sent to {To}", to);
                }
                else
                {
                    // No SMTP configured - save to local folder for review
                    var folder = Path.Combine(Directory.GetCurrentDirectory(), "ContactSubmissions");
                    Directory.CreateDirectory(folder);
                    var fileName = Path.Combine(folder, $"contact_{DateTime.UtcNow:yyyyMMdd_HHmmss}.txt");
                    var content = $"Name: {Input.Name}\nEmail: {Input.Email}\nSubject: {Input.Subject}\nMessage:\n{Input.Message}";
                    await System.IO.File.WriteAllTextAsync(fileName, content);
                    TempData["Success"] = "Your message was saved (SMTP not configured).";
                    _logger.LogInformation("Contact message saved to {File}", fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending contact message");
                ModelState.AddModelError(string.Empty, "An error occurred while sending your message. Please try again later.");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
