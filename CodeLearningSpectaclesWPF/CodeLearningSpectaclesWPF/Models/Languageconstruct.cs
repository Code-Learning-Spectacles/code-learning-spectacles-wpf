using System;
using System.Collections.Generic;

namespace CodeLearningSpectaclesWPF.Models;

public partial class Languageconstruct
{
    public int Languageconstructid { get; set; }

    public int Codinglanguageid { get; set; }

    public int Codeconstructid { get; set; }

    public string Construct { get; set; } = null!;


}
