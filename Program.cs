using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

//Main Program
public class Program
{
	public static void Main()
	{
		DataOperation dataOperation = new DataOperation();
		List<int> upperRight = dataOperation.readData().TrimEnd().Split(' ').ToList().Select(arTemp => Convert.ToInt32(arTemp)).ToList();
		
		Plateau marsArea;
		
		if (upperRight.Count() == 2)
		{
			marsArea = new Plateau(upperRight[0],upperRight[1]);
		}
		else
		{ 
			dataOperation.writeData("Upper Right Coorinates are missing!");
			return;
		}
		List<Rover> rovers = new List<Rover>();
		while (true)
		{
			var roverInfo = dataOperation.readData().TrimEnd().Split(' ').ToList();
			if (roverInfo.Count() == 3)
			{
				int x = Convert.ToInt32(roverInfo[0]);
				int y = Convert.ToInt32(roverInfo[1]);
				string ort = roverInfo[2];
				// Add rover only if position is available
				if (marsArea.isPositionAvail(x,y))
				{
				   Rover r1 = new Rover(x, y, ort);
				   r1.route = dataOperation.readData();
				   rovers.Add(r1);
				}
			}
			else
			{
				// there are no more Rovers or input error
				break;
			}
		}

		Business missionNasa = new Business();
		foreach (Rover r in rovers)
		{
			missionNasa.processMessage(r, marsArea);
			dataOperation.writeData(r.ToString());
		}
		
		UnitTest t = new UnitTest();
		t.runAllTests();
	}
}

// Data Operations (Database/File/Console)
public class DataOperation
{
	public string readData()
	{
		return Console.ReadLine();
	}

	public void writeData(string data)
	{
		Console.WriteLine(data);
	}
	
	public static void writeLog(string log)
	{
		Console.WriteLine("Log:" + log);
	}
}

// Data Classes
public class Rover
{
	public Position currentPos
	{
		get;
		set;
	}

	public string route
	{
		get;
		set;
	}

	public Rover(int _x, int _y, string _orientation)
	{
		currentPos = new Position(_x, _y, _orientation);
	}

	public override string ToString()
	{
		return currentPos.x + " " + currentPos.y + " " + currentPos.o;
	}
}

public enum Orientation
{
	N = 0,
	E = 1,
	S = 2,
	W = 3
};

public class Position
{
	public int x
	{
		get;
		set;
	}

	public int y
	{
		get;
		set;
	}

	public Orientation o
	{
		get;
		set;
	}

	public Position(int _x, int _y, string _orientation)
	{
		x = _x;
		y = _y;
		o = (Orientation)Enum.Parse(typeof(Orientation), _orientation.ToUpper());
	}
}

public class Plateau
{
	Position maxPosition;
	Position minPosition;
	List<Rover> roverList;
	
	public Plateau(int maxX, int maxY)
	{
		minPosition = new Position(0, 0, "N");
		maxPosition = new Position(maxX, maxY, "N");
		roverList = new List<Rover>();
	}
	
	public void addNewRover(Rover r)
	{
		roverList.Add(r);
	}
	
	public bool isPositionAvail(int x, int y)
	{
		if(x > maxPosition.x || y > maxPosition.y || 
		   x < minPosition.x || y < minPosition.y)
		{
			DataOperation.writeLog("Location is out o Plateau!");
			return false;
		}
		
		foreach(Rover r in roverList)
		{
			if (r.currentPos.x == x && r.currentPos.y == y)
			{
				DataOperation.writeLog("Crash Alert - Location is not avaliable!");
				return false;
			}
		}
		return true;
	}
}

// Business operations
public class Business
{
	public void processMessage(Rover r, Plateau marsPlateau)
	{
		string route = r.route.ToUpper();
		Position currentPos = r.currentPos;
		for (int i = 0; i < route.Length; i++)
		{
			switch (route[i])
			{
				case 'R':
				case 'L':
					currentPos.o = turn(currentPos.o, route[i]);
					break;
				case 'M':
					move(currentPos, marsPlateau);
					break;
				default:
					DataOperation.writeLog("Wrong Control String:" + route[i]);
					break;
			}
		}
		
		// After finishing move rover added to Plateau to avoide Rover Crash
		marsPlateau.addNewRover(r);
	}

	public void move(Position p, Plateau map)
	{
		switch (p.o)
		{
			case Orientation.N:
				if (map.isPositionAvail(p.x, p.y + 1))
				{
					p.y += 1;
				}
				break;
			case Orientation.E:
				if (map.isPositionAvail(p.x + 1, p.y))
				{
					p.x += 1;
				}
				break;
			case Orientation.S:
				if (map.isPositionAvail(p.x, p.y - 1))
				{
					p.y -= 1;
				}
				break;
			case Orientation.W:
				if (map.isPositionAvail(p.x - 1, p.y))
				{
					p.x -= 1;
				}
				break;
		}
	}

	public Orientation turn(Orientation o, char newWay)
	{
		int newOrientatiton = (int)o;
		int spinWay = (newWay == 'R' ? 1 : -1);
		newOrientatiton += spinWay;
		newOrientatiton = newOrientatiton % 4;
		// Because of the negative results for mode operation
		newOrientatiton = (newOrientatiton < 0 ? (newOrientatiton + 4) : newOrientatiton);
		return (Orientation)newOrientatiton;
	}
}

//Tests
[TestClass]
public class UnitTest
{
	[TestMethod]
	public void runAllTests()
	{
		testTurnFromNtoRight();
		testTurnFromEtoRight();
		testTurnFromStoRight();
		testTurnFromWtoRight();
		testTurnFromNtoLeft();
		testTurnFromEtoLeft();
		testTurnFromStoLeft();
		testTurnFromWtoLeft();
		
		testMoveFromN();
		testMoveFromE();
		testMoveFromS();
		testMoveFromW();
		testMoveOut();
	}
	
	[TestMethod]
	public void testTurnFromNtoRight()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.E, b.turn(Orientation.N, 'R'));
	}

	[TestMethod]
	public void testTurnFromEtoRight()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.S, b.turn(Orientation.E, 'R'));
	}

	[TestMethod]
	public void testTurnFromStoRight()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.W, b.turn(Orientation.S, 'R'));
	}

	[TestMethod]
	public void testTurnFromWtoRight()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.N, b.turn(Orientation.W, 'R'));
	}

	[TestMethod]
	public void testTurnFromNtoLeft()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.W, b.turn(Orientation.N, 'L'));
	}

	[TestMethod]
	public void testTurnFromEtoLeft()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.N, b.turn(Orientation.E, 'L'));
	}

	[TestMethod]
	public void testTurnFromStoLeft()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.E, b.turn(Orientation.S, 'L'));
	}

	[TestMethod]
	public void testTurnFromWtoLeft()
	{
		Business b = new Business();
		Assert.AreEqual(Orientation.S, b.turn(Orientation.W, 'L'));
	}
	
	[TestMethod]
	public void testMoveFromN()
	{
		Business b = new Business();
		Position p = new Position(0, 0, "N");
		Position controlPos = new Position(0, 1, "N");
		Plateau plat = new Plateau(7, 7);
		b.move(p, plat);
		Assert.AreEqual(controlPos.x, p.x);
		Assert.AreEqual(controlPos.y, p.y);
		Assert.AreEqual(controlPos.o, p.o);
	}
	
	[TestMethod]
	public void testMoveFromE()
	{
		Business b = new Business();
		Position p = new Position(1, 0, "E");
		Position controlPos = new Position(2, 0, "E");
		Plateau plat = new Plateau(2, 3);
		b.move(p, plat);
		Assert.AreEqual(controlPos.x, p.x);
		Assert.AreEqual(controlPos.y, p.y);
		Assert.AreEqual(controlPos.o, p.o);
	}
	
	[TestMethod]
	public void testMoveFromS()
	{
		Business b = new Business();
		Position p = new Position(2, 2, "S");
		Position controlPos = new Position(2, 1, "S");
		Plateau plat = new Plateau(2, 2);
		b.move(p, plat);
		Assert.AreEqual(controlPos.x, p.x);
		Assert.AreEqual(controlPos.y, p.y);
		Assert.AreEqual(controlPos.o, p.o);
	}
	
	[TestMethod]
	public void testMoveFromW()
	{
		Business b = new Business();
		Position p = new Position(2, 3, "W");
		Position controlPos = new Position(1, 3, "W");
		Plateau plat = new Plateau(4, 3);
		b.move(p, plat);
		Assert.AreEqual(controlPos.x, p.x);
		Assert.AreEqual(controlPos.y, p.y);
		Assert.AreEqual(controlPos.o, p.o);
	}
	
	[TestMethod]
	public void testMoveOut()
	{
		Business b = new Business();
		Position p = new Position(5, 5, "N");
		Position controlPos = new Position(5, 5, "N");
		Plateau plat = new Plateau(4, 3);
		b.move(p, plat);
		Assert.AreEqual(controlPos.x, p.x);
		Assert.AreEqual(controlPos.y, p.y);
		Assert.AreEqual(controlPos.o, p.o);
	}
}
