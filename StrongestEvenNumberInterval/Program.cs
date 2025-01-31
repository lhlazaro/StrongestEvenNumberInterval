namespace StrongestEvenNumberInterval
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
          
            try
            {
            
                StrongnessRangeNumber strongestNumber = new StrongnessRangeNumber(5, 10);
                strongestNumber.CalculateStrongestNumber();
                strongestNumber.GenerateTemplateLabelResults();
                Console.WriteLine(strongestNumber.GetTemplateResult());
            
            }
            catch (InvalidRangeException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Class to generate the Strongness numbers from a range
        /// </summary>
        public class StrongnessRangeNumber
        {
            //Initial Range
            public int StartRange { get; }
            public int EndRange { get; }
            public int[] MaxStrongnessNumber { get; private set; }

            //Strongness helpers
            private readonly Dictionary<int, int> Strongness = [];
            private readonly Dictionary<int, int[]> StrongnessGrouped = [];

            //Results string templates
            private readonly string TemplateTitle = "[{{StartRange}}, {{EndRange}}]\t--> {{MaxStrongnessNumber}} \t# ";
            public string TemplateTitleResult { get; private set; }

            private readonly string TemplateBody = "{{SequenceNumbers}} {{Verb}} strongness {{Strongness}}; ";
            public string TemplateBodyResult { get; private set; }
            private readonly string HasVerb = "has";
            private readonly string HaveVerb = "have";

            /// <summary>
            /// constructor to set the range of the Strongness numbers
            /// </summary>
            /// <param name="start">Start Range value</param>
            /// <param name="end">End Range value</param>
            /// <exception cref="InvalidRangeException"></exception>
            public StrongnessRangeNumber(int start, int end)
            {
                StartRange = start;
                EndRange = end;
                if (ValidateContraints())
                    throw new InvalidRangeException();

                MaxStrongnessNumber = [];
                TemplateTitleResult = "";
                TemplateBodyResult = "";
            }
            
            /// <summary>
            /// Validation for the constraints of the range
            /// </summary>
            /// <returns>True in case all good, false if any constrains is not valid</returns>
            private bool ValidateContraints()
            {
                return !(1 <= StartRange
                    && StartRange <= EndRange
                    && EndRange <= int.MaxValue);
            }

            /// <summary>
            /// Calculate the strongness numbers in the range
            /// </summary>
            public void CalculateStrongestNumber()
            {
                for (int i = StartRange; i <= EndRange; i++)
                {
                    Strongness
                        .Add(i, StrongnessEven(i, 0));
                }
            }

            /// <summary>
            /// Primary recursive function to calculate the strongness of a number
            /// </summary>
            /// <param name="number">the number / 2 to find the strongness</param>
            /// <param name="strongnessLoop">current strongness value</param>
            /// <returns>The strongness of number</returns>
            private static int StrongnessEven(int number, int strongnessLoop)
            {
                if (number % 2 != 0)
                {
                    return strongnessLoop;
                }
                return StrongnessEven(number / 2, ++strongnessLoop);
            }

            /// <summary>
            /// Group results to avoid duplicates from strongness
            /// and set max strongness number
            /// </summary>
            private void GroupByStrongness()
            {
                var groupByStrongness = Strongness
                    .GroupBy(x => x.Value);

                foreach (var group in groupByStrongness)
                {
                    StrongnessGrouped
                        .Add(group.Key, group
                            .Select(x => x.Key)
                        .ToArray());
                }

                MaxStrongnessNumber = StrongnessGrouped
                    .OrderByDescending(x => x.Key)
                    .First()
                    .Value;
            }

            /// <summary>
            /// Results string templates
            /// </summary>
            /// <returns>A string with result format</returns>
            private string GenerateTemplateTitle()
            {
                return TemplateTitle
                            .Replace("{{" + nameof(StartRange) + "}}", StartRange.ToString())
                            .Replace("{{" + nameof(EndRange) + "}}", EndRange.ToString())
                            .Replace("{{" + nameof(MaxStrongnessNumber) + "}}", string.Join(",", MaxStrongnessNumber));
            }

            /// <summary>
            /// Results string templates
            /// </summary>
            /// <param name="strongnessGroup">the key value for strongness and numbers</param>
            /// <returns>A string with result format</returns>
            private string GenerateTemplateBody(KeyValuePair<int, int[]> strongnessGroup)
            {
                return TemplateBody
                        .Replace("{{SequenceNumbers}}", string.Join(", ", strongnessGroup.Value))
                        .Replace("{{Verb}}", strongnessGroup.Value.Length == 1 ? HasVerb : HaveVerb)
                        .Replace("{{Strongness}}", strongnessGroup.Key.ToString());
            }
            
            /// <summary>
            /// Generate the template strings results
            /// </summary>
            public void GenerateTemplateLabelResults()
            {
                GroupByStrongness();
                TemplateTitleResult = GenerateTemplateTitle();
                foreach (KeyValuePair<int, int[]> strongnessGroup in StrongnessGrouped)
                {
                    TemplateBodyResult += GenerateTemplateBody(strongnessGroup);
                }
            }

            /// <summary>
            /// Get the template results
            /// </summary>
            /// <returns></returns>
            public string GetTemplateResult()
            {
                return TemplateTitleResult + TemplateBodyResult;
            }
        }

        /// <summary>
        /// Exception Class for invalid range in Strongness numbers
        /// </summary>
        public class InvalidRangeException : Exception
        {
            public InvalidRangeException(string message = "Invalid Range for Strongeness: StartRange should be greater than 1, StartRange should be less than EndRange, EndRange should be less than int.MaxValue ") : base(message) { }
        }
    }
}