using System;
using System.Collections.Generic;
using System.IO;

namespace CreateSQLScript
{
  class Program
  {
    static void Main()
    {
      Action<string> display = Console.WriteLine;
      display("Application to create a script.sql file to rename all constraints written for .NET Core 3.1");
      List<string> allConstraints = new List<string>();
      try
      {
        using (StreamReader streamReader = new StreamReader("liste_constraints.txt"))
        {
          while (streamReader.Peek() >= 0)
          {
            allConstraints.Add(streamReader.ReadLine());
          }
        }
      }
      catch (Exception)
      {
        display("exception while reading the constraint file");
      }

      // remove last 10 characters
      List<string> allConstraints2 = new List<string>();
      foreach (string item in allConstraints)
      {
        allConstraints2.Add(item.Substring(0, item.Length - 10));
      }

      // insert line
      // sp_rename 'DF__table__column__1978F273', 'DF__table__column'
      // sp_rename 'DF__BSMV__idParticul__60207986', 'DF__BSMV__idParticul';
      List<string> allConstraints3 = new List<string>();
      Dictionary<string, string> dicoConstraints = new Dictionary<string, string>();
      Dictionary<string, string> inverseDicoConstraints = new Dictionary<string, string>();
      for (int i = 0; i < allConstraints.Count; i++)
      {
        allConstraints3.Add($"sp_rename '{allConstraints[i]}', '{allConstraints2[i]}';");
        // rename duplicate with a number 2
        if (inverseDicoConstraints.ContainsKey(allConstraints2[i]))
        {
          inverseDicoConstraints.Add($"{allConstraints2[i]}2", allConstraints[i]);
        }
        else
        {
          inverseDicoConstraints.Add(allConstraints2[i], allConstraints[i]);
        }
      }

      // reverse dictionary
      foreach (var item in inverseDicoConstraints)
      {
        dicoConstraints.Add(item.Value, item.Key);
      }

      List<string> allConstraints4 = new List<string>();
      for (int i = 0; i < allConstraints.Count; i++)
      {
        allConstraints4.Add($"{allConstraints3[i]}{Environment.NewLine}GO{Environment.NewLine}");
      }

      try
      {
        using (StreamWriter streamWriter = new StreamWriter("script.sql"))
        {
          foreach (string item in allConstraints3)
          {
            streamWriter.WriteLine(item);
          }
        }
      }
      catch (Exception exception)
      {
        display($"exception while trying to write the result file: {exception.Message}");
      }

      display("The file script.sql has been created");
      display("Press any key to exit:");
      Console.ReadKey();
    }
  }
}
