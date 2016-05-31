using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web;

namespace SB004.Models
{

  public class ParsedExternalAccessToken
  {
    public string user_id { get; set; }
    public string app_id { get; set; }
    public string username { get; set; }
  }
  public class NewAccountModel
  {
      [Required(ErrorMessage = "UserName is required")]
	  [StringLength(20, ErrorMessage = "The UserName cannot be more than 20 characters long")]
      public string UserName { get; set; }

      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Invalid Email Address")]
      public string Email { get; set; }

      [Required(ErrorMessage = "Password is required")]
      [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
      [Display(Name = "Password")]
      public string Password { get; set; }
  }
  public class SignInModel
  {
      [Required(ErrorMessage = "Email is required")]
      [EmailAddress(ErrorMessage = "Invalid Email Address")]
      public string Email { get; set; }

      [Required(ErrorMessage = "Password is required")]
      public string Password { get; set; }
  }

	public class AccountDetailsModel
	{
		[StringLength(20, ErrorMessage = "The UserName cannot be more than 20 characters long")]
		public string UserName { get; set; }
		[EmailAddress(ErrorMessage = "Invalid Email Address")]
		public string Email { get; set; }
		public string Password { get; set; }
		public string NewPassword { get; set; }
		public string StatusMessage { get; set; }
	}
}