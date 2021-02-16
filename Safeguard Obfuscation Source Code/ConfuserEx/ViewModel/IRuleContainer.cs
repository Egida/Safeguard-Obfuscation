using System.Collections.Generic;

namespace ConfuserEx.ViewModel
{
  internal interface IRuleContainer
  {
    IList<ProjectRuleVM> Rules { get; }
  }
}
