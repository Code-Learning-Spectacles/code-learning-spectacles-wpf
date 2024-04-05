using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLearningSpectaclesWPF.Models
{
  internal class AccessToken
  {
    public string access_token {  get; set; } = string.Empty;
    public string token_type { get; set; } = string.Empty;
    public string scope { get; set; } = string.Empty;
  }
}
