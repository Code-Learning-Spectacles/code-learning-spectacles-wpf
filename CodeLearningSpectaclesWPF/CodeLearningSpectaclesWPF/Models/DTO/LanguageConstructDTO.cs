using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeLearningSpectaclesWPF.Models.DTO
{
     public class LanguageConstructDTO
    {
        public int Languageconstructid { get; set; }

        public required string Codinglanguage { get; set; }

        public required string Codeconstruct { get; set; }

        public string Construct { get; set; } = null!;

    }


}
