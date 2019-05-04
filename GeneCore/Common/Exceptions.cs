using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace GeneCore.Common
{
    public static class Exceptions
    {
        /// <summary>
        ///     Exception indicating building process was not finished before calling Build.
        /// </summary>
        public class GeneticSelectionProcessNotCompleted : Exception
        {
            [NotNull]
            private readonly IList<String> _missingProperties;

            public GeneticSelectionProcessNotCompleted(params String[] missingProperties)
            {
                _missingProperties = new List<String>();

                if (missingProperties == null) return;

                foreach (String property in missingProperties) _missingProperties.Add(property);
            }

            public override String ToString()
            {
                String msg = "GeneticSelectionProcess being constructed was not completed before Build().";
                if (_missingProperties.Any()) msg += $" Missing properties {string.Join(",", _missingProperties)}";

                return msg;
            }
        }
    }
}