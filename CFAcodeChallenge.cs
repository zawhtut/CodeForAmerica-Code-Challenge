using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Collections.Generic;

public class Program
{
	public class Violation 
	{
	
		public int 		ViolationID 	{get;set;}
		public int 		InspectionID	{get;set;}
		public string 		VCategory 	{get;set;}
		public DateTime 	VDate 		{get;set;}
		public DateTime 	VDateClosed 	{get;set;}
		public string 		VType 		{get;set;}
	}
	
	public static void Main()
	{
		List<Violation> list = new List<Violation>();
		
		var url = "http://forever.codeforamerica.org/fellowship-2015-tech-interview/Violations-2012.csv";
		var client = new WebClient();
		
		//To Enter CSV link manually
		//var url = Console.ReadLine();
		
		using (var stream = client.OpenRead(url))
			
			using (var reader = new StreamReader(stream))
			{
				string 		line;
				string 		TitleLine = reader.ReadLine();
				string[] 	column = new string[6];
				
				//Reading CSV line by line
				while ((line = reader.ReadLine()) != null)
				{
					//Splitting the string of a line into array
					char[] 	ChaArr 	= line.ToCharArray();
					
					int 	j 		= 1;
					int 	k 		= 0;
					
					//Parsing Data from CSV file
					for (int c = 1; c <= ChaArr.Length; c++)
					{
						if (c == ChaArr.Length)
						{
							column[k] = new string (ChaArr, j - 1, c - j + 1);
						}
							else if (ChaArr[c - 1] == ',')
							{
								column[k] = new string (ChaArr, j - 1, c - j);
								j = c + 1;
								k++;
							}
								else if (ChaArr[c - 1] == '"')
								{
									column[k] = new string (ChaArr, c, ChaArr.Length - c - 1);
									break;
								}
					}

					//Creat an Violation data object
					Violation violation = new Violation
					{
						ViolationID 	= Int32.Parse(column[0]), 
						InspectionID 	= Int32.Parse(column[1]), 
						VCategory 	= column[2], 
						VDate 		= Convert.ToDateTime(column[3]), 
						VDateClosed 	= (column[4].Length == 0) ? DateTime.Now : Convert.ToDateTime(column[4]), 
						VType 		= column[5]
					};
					
					//Put it in the array list of the object
					list.Add(violation);
				}

				Console.WriteLine("Total Violations: {0}", list.Count);
				Console.WriteLine();
			}

		//Perform the Query
		var CategoryCounts =
			from p in list
			group p by p.VCategory into g
				select new
				{
					g.Key, 
					count = g.Count(), 
					earliest = g.Min(p => p.VDate), 
					latest = g.Max(p => p.VDate)
				};
		
		//Dispay The Query 
		foreach (var v in CategoryCounts)
		{
			Console.WriteLine("Violation \'{0}\' occurs {1} times.", v.Key, v.count);
			Console.WriteLine("» Earliest Day of the violation : " + v.earliest.ToString("ddd MMM dd, yyyy"));
			Console.WriteLine("» Latest Day of the violation   : " + v.latest.ToString("ddd MMM dd, yyyy"));
			Console.WriteLine();
		}

		Console.WriteLine("====== THANKS ======");
	}
}
