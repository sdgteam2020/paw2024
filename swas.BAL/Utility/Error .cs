namespace swas.BAL.Utility
{
    public class Error
    {
        

        public static int ExceptionHandle(string message)
        {
            string docPath = "wwwroot/Error";
            string filePath = Path.Combine(docPath, "Error.txt");
            List<string> lines = File.ReadAllLines(filePath).ToList();
            lines.Add("-----------------------------------------------------------------------------");
            lines.Add("Date : " + DateTime.Now.ToString());
            lines.Add(message);
            if (lines.Count > 900)
            {
                int startIndex = lines.Count - 900; // Keep the most recent 200 lines
                lines = lines.Skip(startIndex).ToList();
            }
            File.WriteAllLines(filePath, lines);

            return 0;
        }


    }
}
