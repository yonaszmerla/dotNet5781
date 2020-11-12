﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace dotNet5781_02_2375_6415
{
    using static Math;
    /// <summary>
    /// A new exception class for cases where we didn't find the product that the user entered
    /// </summary>
    [Serializable]
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() { }
        public NotFoundException(string message) : base(message) { }
        public NotFoundException(string message, Exception inner) : base(message, inner) { }
        protected NotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
    /// <summary>
    /// A class that represents a bus stop
    /// </summary>
    class BusStop
    {
        /// <summary>
        /// empty CTOR
        /// </summary>
        /// <param name="number">number of the station</param>
        /// <param name="tmpAddress">address of the station</param>
        public BusStop(int number = 0, string tmpAddress = "")
        {
            busStationKey = number;
            address = tmpAddress;
            Random r = new Random(DateTime.Now.Millisecond);
            longitude = ((float)r.NextDouble() * ((float)1.2)) + (float)34.3; // Lottery of longitude within the coordinates of the State of Israel
            latitude = ((float)r.NextDouble() * ((float)2.3)) + (float)31; // Lottery of latitude within the coordinates of the State of Israel
        }
        /// <summary>
        /// number of the station
        /// </summary>
        protected int busStationKey;
        /// <summary>
        /// getter & setter for the number of the station
        /// </summary>
        public int BusStationKey
        {
            get { return busStationKey; }
            set { busStationKey = value; }
        }
        /// <summary>
        /// the station's latitude
        /// </summary>
        protected float latitude;
        /// <summary>
        /// getter for the station's latitude
        /// </summary>
        public float Latitude
        {
            get { return latitude; }
        }
        /// <summary>
        /// the station's longitude
        /// </summary>
        protected float longitude;
        /// <summary>
        /// getter for the station's Longitude
        /// </summary>
        public float Longitude
        {
            get { return longitude; }
        }
        /// <summary>
        /// the station's address
        /// </summary>
        protected string address;
        /// <summary>
        /// getter & setter for the station's address
        /// </summary>
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        /// <summary>
        /// overriding ToString func which print out the station details
        /// </summary>
        /// <returns>string with all the details</returns>
        public override string ToString()
        {
            string tmpString = "Bus Station Code:  " + busStationKey.ToString() + ", " + latitude.ToString() + "°N " + longitude.ToString() + "°E";
            return tmpString;
        }
    }
    /// <summary>
    /// A class that will represent a bus line station.
    /// Contains all bus station data,
    /// and also contains the distance from the previous bus line station,
    /// and the travel time from the previous bus line station
    /// </summary>
    class BusLineStop : BusStop
    {
        /// <summary>
        /// empty CTOR
        /// </summary>
        /// <param name="number">number of the station</param>
        /// <param name="tmpAddress">address of the station</param>
        public BusLineStop(int number = 0, string tmpAddress = "") : base(number, tmpAddress) { }


        private double distance = 0;

        public double Distance
        {
            get { return distance; }
            set { distance = value; }
        }
        /// <summary>
        /// Calculates distance using coordinates (found on internet)
        /// </summary>
        /// <param name="tmpBus">The previous station</param>
        public void SetDistance(BusLineStop tmpBus)
        {
            double theta = tmpBus.longitude - longitude;
            distance = (Math.Sin((tmpBus.latitude * Math.PI) / 180.0) * Math.Sin((latitude * Math.PI) / 180.0) + Math.Cos((tmpBus.longitude * Math.PI) / 180.0) * Math.Cos((longitude * Math.PI) / 180.0) * Math.Cos((theta * Math.PI) / 180.0));
            distance = Math.Acos(distance);
            distance = (distance * 60 * 1.1515 * 1.609344);
        }


        private TimeSpan travelTime = new TimeSpan(0,0,0);

        public TimeSpan TravelTime
        {
            get { return travelTime; }
            set { travelTime = value; }
        }

        /// <summary>
        /// Calculates the travel time from the previous station.
        /// assuming that the bus runs on average at a speed of 40 km / h
        /// </summary>
        /// <param name="tmpBus">The previous station</param>
        /// <returns>The duration of the journey from the previous station as a show of TimeSpan</returns>
        public void SetTravelTime()
        {
            // Calculation of hours, minutes and seconds according to a rate of 40 km / h and depending on the distance
            travelTime = new TimeSpan((int)(distance / 40.0), (int)((distance % 40.0) / (40.0 / 60.0)), (int)(((distance % 40.0) % (40.0 / 60.0)) / (40.0 / 3600.0)));
        }
    }
    /// <summary>
    /// Assigning the bus line to a specific area from a defined area list
    /// or be cross-areas (general)/// </summary>
    enum Area { General, North, South, Center, Jerusalem };
    /// <summary>
    /// A class that will represent a single bus line
    /// which is defined as a route of various bus line stations/// </summary>
    class Line : IComparable<Line>, IEnumerable
    {
        /// <summary>
        /// CTOR
        /// </summary>
        /// <param name="number">the line number</param>
        /// <param name="tmpArea">the line area</param>
        public Line(int number = 0, Area tmpArea = Area.General)
        {
            lineNumber = number;
            busArea = tmpArea;
            stations = new List<BusLineStop>();
        }
        /// <summary>
        /// the line number
        /// </summary>
        private int lineNumber;
        /// <summary>
        /// getter & setter for the line number
        /// </summary>
        public int LineNumber
        {
            get { return lineNumber; }
            set { lineNumber = value; }
        }
        /// <summary>
        /// The first stop the line passes through
        /// </summary>
        private BusLineStop firstStation;
        /// <summary>
        /// getter & setter for the first station
        /// </summary>
        public BusLineStop FirstStation
        {
            get { return firstStation; }
            set { firstStation = value; }
        }
        /// <summary>
        /// The last stop the line passes through
        /// </summary>
        private BusLineStop lastStation;
        /// <summary>
        /// getter & setter for the last station
        /// </summary>
        public BusLineStop LastStation
        {
            get { return lastStation; }
            set { lastStation = value; }
        }
        /// <summary>
        /// The area in which the line travels
        /// </summary>
        private Area busArea;
        /// <summary>
        /// getter & setter for the area
        /// </summary>
        public Area BusArea
        {
            get { return busArea; }
            set { busArea = value; }
        }
        /// <summary>
        /// List of bus stops where the line stops
        /// </summary>
        List<BusLineStop> stations;
        /// <summary>
        /// overriding ToString func which print out the line details
        /// including the line number, the area where the line operates and the list of station numbers
        /// </summary>
        /// <returns>string with all the details</returns>
        public override string ToString()
        {
            string tmpString = "BusLine:  " + lineNumber.ToString() + ", " + busArea.ToString() + " / ";
            for (int i = 0; i < stations.Count; i++) // add to the string the list of station numbers
            {
                tmpString += stations[i].BusStationKey.ToString();
                tmpString += " ";
            }

            return tmpString;
        }
        /// <summary>
        /// Adds station in line
        /// if Index out of range throw ArgumentOutOfRangeException
        /// if station already exists throw ArgumentException
        /// </summary>
        /// <param name="predStation">Key of last station in the list of stations</param>
        /// <param name="stationNum">the number of the station</param>
        /// <param name="tmpAdress">the address of the station</param>
        public void AddStation(int predStation, int stationNum, string tmpAdress = "")
        {
            int index = 0; //index where to insert
            if (predStation != 0) //if not first stop
            {
                for (; index < stations.Count; index++) //looks for index 
                {
                    if (stations[index].BusStationKey == predStation)
                    {
                        break;
                    }
                }
                if (index > stations.Count + 1) // if index out of range
                {
                    throw new ArgumentOutOfRangeException("index");
                }
            }
            if (!CheckStation(stationNum)) // If the station does not exist on the route
            {
                BusLineStop tmpStation = new BusLineStop(stationNum, tmpAdress);
                if (index < stations.Count)
                {
                    if (index == 0) //if adds first stop
                    {
                        firstStation = tmpStation; //update first stop
                        stations.Insert(index, tmpStation); //adds station
                        stations[index + 1].SetDistance(stations[index]);
                        stations[index + 1].SetTravelTime();
                    }
                    else
                    {
                        stations.Insert(index, tmpStation); //adds station
                        stations[index].SetDistance(stations[index - 1]);
                        stations[index].SetTravelTime();
                        stations[index + 1].SetDistance(stations[index]);
                        stations[index + 1].SetTravelTime();
                    }
                }
                else if (index == stations.Count) //if adds last stop
                {
                    if (index != 0) //if last stop isn't first stop
                    {
                        stations.Add(tmpStation); //adds station
                        lastStation = tmpStation; //update last stop
                        stations[index].SetDistance(stations[index - 1]);
                        stations[index].SetTravelTime();
                    }
                    else //if last station is first station
                    {
                        firstStation = tmpStation; //update first stop
                        stations.Add(tmpStation); //adds station
                        lastStation = tmpStation; //update last stop
                    }
                }
            }
            else // ERROR: Bus station already exists, bus can pass by the same station only once
            {
                throw new ArgumentException("Bus station already exists, bus can pass by the same station only once ");
            }
        }
        /// <summary>
        /// Adds station in line
        /// if Index out of range throw ArgumentOutOfRangeException
        /// if station already exists throw ArgumentException
        /// </summary>
        /// <param name="index">Index of station in line</param>
        /// <param name="newStop">Station</param>
        //public void AddStation(int index, BusLineStop newStop)
        //{
        //    try
        //    {
        //        if (index > stations.Count + 1)// if index out of range
        //        {
        //            ArgumentOutOfRangeException ex = new ArgumentOutOfRangeException("index");
        //            throw ex;
        //        }
        //    }
        //    catch (ArgumentOutOfRangeException ex)
        //    {
        //        Console.WriteLine($" ERROR : {ex.ToString()}");
        //    }
        //    if (!CheckStation(newStop.BusStationKey)) //if station doesn't exist
        //    {
        //        if (index <= stations.Count) 
        //        {
        //            if (index == 1) //if adds first stop
        //            {
        //                firstStation = newStop; //update first stop
        //            }
        //            stations.Insert(index - 1, newStop); //adds station
        //        }
        //        else if (index == stations.Count + 1) //if adds last stop
        //        {
        //            if (index != 1) //if last stop isn't first stop
        //            {
        //                stations.Add(newStop); //adds station
        //                lastStation = newStop; //updates last station
        //            }
        //            else //if last station is first station
        //            {
        //                firstStation = newStop; //updates first station
        //                stations.Add(newStop); //adds station
        //                lastStation = newStop; //updates last station
        //            }
        //        }
        //    }
        //    else // ERROR: Bus station already exists, bus can pass by the same station only once
        //    {
        //        throw new ArgumentException("Bus station already exists, bus can pass by the same station only once ");
        //    }
        //}
        /// <summary>
        /// Deletes station in line
        /// If station doesn't exist throws NotFoundException
        /// </summary>
        /// <param name="stationNum">Number of station</param>
        public void DeleteStation(int stationNum)
        {
            int i = 0;
            for (; i < stations.Count; i++) //goes over the all line
            {
                if (stations[i].BusStationKey == stationNum) //if found
                {
                    break;
                }
            }
            if (i >= stations.Count) //if not found
            {
                throw new NotFoundException("ERROR : Bus stop not found !!");
            }
            if ((i - 1) != stations.Count) //if not last station
            {
                if (i == 0) //if it is the first station
                {
                    firstStation = stations[0]; //update first station
                    stations[i + 1].Distance = 0;
                    stations[i + 1].SetTravelTime();
                }
                else
                {
                    stations[i + 1].SetDistance(stations[i - 2]);
                    stations[i + 1].SetTravelTime();
                }
            } 
            else  //if it is last station
            {
                lastStation = stations[i - 1]; //update last station
            }
            stations.RemoveAt(i); //remove the station at found index
        }
        /// <summary>
        /// Checks if station exists in line and returns boolean
        /// </summary>
        /// <param name="stationNum">Number of station</param>
        /// <returns></returns>
        public bool CheckStation(int stationNum)
        {
            for (int i = 0; i < stations.Count; i++) //goes over the all list
            {
                if (stations[i].BusStationKey == stationNum) //if found
                {
                    return true;
                }
            }
            return false; //else
        }

        /// <summary>
        /// Calculates Distance between 2 stops and returns it
        /// </summary>
        /// <param name="stop1">Source's station</param>
        /// <param name="stop2">Destination's station</param>
        /// <returns>Returns distance</returns>
        //public double SetDistance(BusLineStop stop2)
        //{
        //int i = 0;
        //for (; i < stations.Count; i++)//searches for first station
        //{
        //    if (stations[i].BusStationKey == stop1)
        //    {
        //        break;
        //    }
        //}
        //if ((i == stations.Count) || !CheckStation(stop2)) //if doesn't find first or second stop
        //{
        //    throw new NotFoundException("Cannot calculate duration, route doesn't exist in line !!");

        //    do
        //    {
        //        i++;
        //        distance += stations[i].Distance(stations[i - 1]); ///sums distance from first stop to second stop
        //    } while (stations[i].BusStationKey != stop2);
        //    return distance;
        //}
        /// <summary>
        /// Calculates the time of route between 2 stations
        /// </summary>
        /// <param name="stop1">Source's station</param>
        /// <param name="stop2">Destination's station</param>
        /// <returns>Return time in TimeSpan Form</returns>
        public TimeSpan Time(int stop1, int stop2)
        {
            int i = 0;
            for (; i < stations.Count; i++) //searches for first stop
            {
                if (stations[i].BusStationKey == stop1) //found
                {
                    break;
                }
            }
            if ((i == stations.Count) || !CheckStation(stop2)) //if doesn't find first or second stop
            {
                throw new NotFoundException("Cannot calculate duration, route doesn't exist in line !!");
            }
            TimeSpan time = new TimeSpan(); //creates new TimeSpan
            do
            {
                time += stations[++i].TravelTime; //sums time from first station to least station using time function
            } while (stations[i].BusStationKey != stop2);
            return time;
        }
        /// <summary>
        /// creates a subline composed by the route between 2 stops
        /// </summary>
        /// <param name="stop1">Source's stop</param>
        /// <param name="stop2">Destination's stop</param>
        /// <returns>Return a subLine</returns>
        public Line SubLine(int stop1, int stop2)
        {
            int i = 0;
            bool flag = false; //checks if stations found
            for (; i < stations.Count; i++) //goes over the all line
            {
                if (stations[i].BusStationKey == stop1) //if station found
                {
                    flag = true; //first station was found
                    break;
                }
            }
            if (flag == true) //if first station was found
            {
                for (int j = i; j < (stations.Count); j++) //goes over all remaining line to find second station
                {
                    if (stations[j].BusStationKey == stop2) //if second station
                    {
                        flag = true;//second station found
                        break;
                    }
                    flag = false; //didn't find second station
                }
            }
            Line subLine = new Line(); //creates a new subLine
            if (flag == false) //if 2 stations not found
            {
                subLine = null;
                return subLine; //returns a null subline
            }
            subLine.firstStation = stations[i]; //updates first station of subLine
            do
            {
                subLine.stations.Add(stations[i]); //adds stations to subLine until reaches the second station
            } while (stations[i++].BusStationKey != stop2);
            subLine.LastStation = stations[i - 1]; //updates last station of subLine
            return subLine;
        }
        /// <summary>
        /// Finds station in line and returns it
        /// </summary>
        /// <param name="number">Number of the stataion</param>
        /// <returns>Returns the station found</returns>
        public BusLineStop FindStation(int number)
        {
            BusLineStop tmpStop = new BusLineStop(-1); //creataes a new station with number -1 (not found)
            foreach (BusLineStop item in stations) //goes over all stations in list
            {
                if (item.BusStationKey == number) //if found
                {
                    return item; //return the station found
                }
            }
            return tmpStop; //else return a -1 station (not found)
        }
        /// <summary>
        /// Implementation of function compareTo of Icomparable interface
        /// </summary>
        /// <param name="line2">Line to compare with</param>
        /// <returns></returns>
        public int CompareTo(Line line2)
        {
            //compares the time of the 2 lines
            TimeSpan time1 = Time(firstStation.BusStationKey, LastStation.BusStationKey);
            TimeSpan time2 = line2.Time(line2.firstStation.BusStationKey, line2.lastStation.BusStationKey);
            return time1.CompareTo(time2); //return comparaison of time
        }
        /// <summary>
        /// Implemantation of enumerator interface
        /// </summary>
        /// <returns>Returns enumarator of the list of stations</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return stations.GetEnumerator();
        }

    }
    /// <summary>
    /// Class that implement a list of lines of bus
    /// </summary>
    class BusLinesList : IEnumerable
    {
        private List<Line> myList = new List<Line> { };
        /// <summary>
        /// Add line to lines list
        /// </summary>
        /// <param name="lineNumber">Number of the line</param>
        /// <param name="tmpArea">Area of the line</param>
        /// <returns></returns>
        public void AddLine(int lineNumber, Area tmpArea = Area.General)
        {
            int i = 0;
            foreach (var Line in myList) //goes over all lines
            {
                if (Line.LineNumber == lineNumber) //if same number
                {
                    i++; //sums the number of lines that have the same number than the one we want to add
                }
            }
            if (i < 2) //if there is less than 2 lines with this number
            {
                Line tmpLine = new Line(lineNumber, tmpArea); //create new line
                myList.Add(tmpLine); //add line to list
            }
            else //if there is already 2 lines
            {
                throw new ArgumentException("Line already exists, cannot build more than 2 routes for the same line !!");
            }
        }
        /// <summary>
        /// Add station to a line in the list
        /// </summary>
        /// <param name="lineNumber">Line number</param>
        /// <param name="index">Index of station in line</param>
        /// <param name="stationNumber">number of station</param>
        /// <param name="stationAddress">Adress of the station</param>
        public void AddStation(int lineNumber,int predStation, int stationNumber, string stationAddress = "")
        {
            //bool flag = false; //checks if line exists
            BusLinesList tmpList = this[lineNumber];
            if (tmpList == null)
            {
                throw new NotFoundException("Bus line not found !!");
            }
            int lineChoice;
            if (tmpList.myList.Count == 2)
            {
                Console.WriteLine("First way - 1 / second way - 2");
                lineChoice = Program.getIntInput();
                tmpList.myList[lineChoice - 1].AddStation(predStation, stationNumber);
            }
            else
            {
                tmpList.myList[0].AddStation(predStation, stationNumber);
            }
            //foreach (Line item in myList)//goes over all lines
            //{
            //    if (item.LineNumber == lineNumber)//if the line to add
            //    {
            //        flag = true;//line exists
            //        item.AddStation(index, stationNumber); //add a new station with this number to the line
            //        break; //end
            //    }
            //}
            //if (!flag) //if line doesn't exist
            //{
            //    throw new NotFoundException("Bus line not found !!");
            //}
            //BusLineStop tmpStop = FindStop(stationNumber); //searches if the bus stop already exists and get it
            //if (tmpStop.BusStationKey != -1) //if bus station already exists
            //{
            //    bool flag = false; //checks if line exists
            //    foreach (Line item in myList) //goes over all lines
            //    {
            //        if (item.LineNumber == lineNumber) //if the line to add
            //        {
            //            flag = true; //line exists
            //            item.AddStation(index, tmpStop); //add the station found to this line
            //            break; //end
            //        }
            //    }
            //    if (!flag) //if line doesn't exist
            //    {
            //        throw new NotFoundException("Bus line not found !!");
            //    }
            //}
            //else
            //{
               
            //}
        }
        /// <summary>
        /// Deletes line in lines list
        /// </summary>
        /// <param name="tmpLine">Number of the line to delete</param>
        public void DeleteLine(int tmpLine)
        {
            int i = 0;
            for (; i < myList.Count; i++) //goes over all lines
            {
                if (myList[i].LineNumber == tmpLine) //if this line has the number to delete
                {
                    myList.RemoveAt(i); //remove this line
                    break; //end
                }
            }
            if (i == myList.Count) //if no line was deleted ( the line didn't exist)
            {
                throw new NotFoundException("Cannot delete line that doesn't exists !!");
            }
        }
        /// <summary>
        /// Checks if station already exists in one of the lines and returns it
        /// </summary>
        /// <param name="number">Number of the station we look for</param>
        /// <returns>Return the station if found else return a station with number -1</returns>
        public BusLineStop FindStop(int number)
        {
            BusLineStop tmpStop = new BusLineStop(); //creates new station
            foreach (Line item in myList) //goes over all lines in list
            {
                tmpStop = item.FindStation(number); //checks if station is in line using function, if yes get it
                if (tmpStop.BusStationKey != -1) //if station is in line
                {
                    return tmpStop;
                }
            }
            return tmpStop;//returns a station with number -1 (not found)
        }
        /// <summary>
        /// Returns list of lines in wich a specific station exists
        /// </summary>
        /// <param name="tmpStation">Number of the station we look for</param>
        /// <returns>returns the subList of lines </returns>
        public List<Line> FindStation(int tmpStation)
        {
            List<Line> subList = new List<Line> { }; //creates new list of lines
            foreach (Line line in myList) //goes over all lines in list
            {
                foreach (BusLineStop busStation in line) //goes over all stations in line
                {
                    if (tmpStation == busStation.BusStationKey) //if station is in line
                    {
                        subList.Add(line); //adds line to the subList
                        break;
                    }
                }
            }
            if (subList.Count == 0) //if subList is empty (no bus line was found)
            {
                throw new NotFoundException("cannot create list of lines, no line deserves that station !!");
            }
            return subList;
        }
        /// <summary>
        /// Creates sublist of Bus lines composed by all routes between 2 stops
        /// </summary>
        /// <param name="stn1">Source's station</param>
        /// <param name="stn2">Destination's station</param>
        /// <returns>returns subList of bus lines</returns>
        public BusLinesList CreateSubList(int stn1, int stn2)
        {
            BusLinesList subList = new BusLinesList();
            foreach (Line line in myList) //goes over all the lines
            {
                Line tmpLine = line.SubLine(stn1, stn2); //gets a subLine if this line has a route from stn1 to stn2
                if (tmpLine != null) //if the subLine exists
                {
                    tmpLine.LineNumber = line.LineNumber; //gets the number of the line from wich this subline is taken
                    subList.myList.Add(tmpLine); //adds subLine to subList
                }
            }
            return subList;
        }
        /// <summary>
        /// Sorts a list of lines according to the default comparer (Time of travel from begin to end)
        /// </summary>
        public void SortList()
        {
            myList.Sort();
        }
        /// <summary>
        /// Implementation of indexer interface
        /// </summary>
        /// <param name="tmpLine">Gets number of line</param>
        /// <returns>Returns List of lines of the number entered</returns>
        public BusLinesList this[int tmpLine]
        {
            get
            {
                BusLinesList tmpList = new BusLinesList(); //creates tmpList of lines to return
                for (int i = 0; i < myList.Count; i++) //searches for line in all array
                {
                    if (myList[i].LineNumber == tmpLine)
                    {
                        tmpList.myList.Add(myList[i]); //adds line in the List
                    }
                }
                if (tmpList.myList.Count == 0)
                {
                    tmpList = null;
                }
                return tmpList;
            }
        }
        /// <summary>
        /// Implementation of enumerator interface
        /// </summary>
        /// <returns>Enumerator of the list</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return myList.GetEnumerator();
        }
    }

    class Program
    {
        enum Choice { ADD = 1, DELETE, FIND, PRINT, EXIT }

        static void Main(string[] args)
        {
            BusLinesList myList = new BusLinesList(); //creates a list of lines
            for (int i = 0; i < 10; i++) //adds lines
            {
                myList.AddLine(i + 1);
            }

            for (int i = 0; i < 10; i++) //adds stations
            {
                int k = 0;
                int tmp = 0;
                for (int j = 0; j < 20; j++)
                {
                    tmp = r.Next(1, 41);
                    try
                    {
                        myList.AddStation(i + 1,k,tmp);
                    }
                    // catches arr empties for the boot continuity of stations and lines and to avoid duplication
                    catch (ArgumentOutOfRangeException ex)
                    {
                    }
                    catch (NotFoundException ex1)
                    {
                    }
                    catch (ArgumentException ex2)
                    {
                    }
                    k = tmp;
                }
            }
            int myChoice; //gets choice of user
            int innerChoice; //gets innerchoice of user
            do
            {
                Console.WriteLine("Enter your choice :");
                Console.WriteLine("Add - 1 / Delete - 2 / Find - 3 / Print - 4 / Exit - 5 ");
                Console.WriteLine();
                myChoice = getIntInput();
                switch ((Choice)myChoice)
                {
                    case Choice.ADD:
                        Console.WriteLine("Enter your choice :");
                        Console.WriteLine("New line - 1 / New BusStop - 2");
                        innerChoice = getIntInput(); // read and check the input
                        if (innerChoice == 1) // adds New line
                        {
                            Console.WriteLine("Enter Number of Line :");
                            innerChoice = getIntInput(); // read and check the input
                            try
                            {
                                myList.AddLine(innerChoice);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine($" ERROR : {ex.ToString()}");
                            }
                            innerChoice = 1;
                        }
                        if (innerChoice == 2) // adds New bus stop
                        {
                            Console.WriteLine("Enter Bus Line :");
                            int tmpNum; // num of the line
                            tmpNum = getIntInput();
                            Console.WriteLine("Enter number of pred Stop in Line :");
                            int index = getIntInput(); // Location of the station on the list

                            Console.WriteLine("Enter number of station :");
                            int stnNum = getIntInput(); // num of station
                            try
                            {
                                myList.AddStation(tmpNum, index, stnNum);
                            }
                            catch (ArgumentOutOfRangeException ex) // wrong index
                            {
                                Console.WriteLine($" ERROR : {ex.ToString()}");
                            }
                            catch (NotFoundException ex1) // no line
                            {
                                Console.WriteLine($" ERROR : {ex1.ToString()}");
                            }
                            catch (ArgumentException ex2) // bus stop already exist
                            {
                                Console.WriteLine($" ERROR : {ex2.ToString()}");
                            }
                        }
                        break;
                    case Choice.DELETE:
                        Console.WriteLine("Enter your choice :");
                        Console.WriteLine("Delete line - 1 / Delete BusStop - 2");
                        innerChoice = getIntInput();
                        if (innerChoice == 1)
                        {
                            Console.WriteLine("Enter Bus Line :");
                            int tmpNum = getIntInput();
                            try
                            {
                                myList.DeleteLine(tmpNum);
                            }
                            catch (NotFoundException ex) // wrong line
                            {
                                Console.WriteLine($" ERROR : {ex.ToString()}");
                            }
                        }
                        if (innerChoice == 2) // Delete BusStop
                        {
                            Console.WriteLine("Enter Bus Line :");
                            int tmpNum = getIntInput();
                            bool flag = false; // true = we find the line
                            foreach (Line line in myList)
                            {
                                if (line.LineNumber == tmpNum)
                                {
                                    flag = true;
                                    Console.WriteLine("Enter number of bus stop in line :");
                                    tmpNum = getIntInput();
                                    try
                                    {
                                        line.DeleteStation(tmpNum);
                                    }
                                    catch (NotFoundException ex) // no exist bus stop in this line
                                    {
                                        Console.WriteLine($" ERROR : {ex.ToString()}");
                                    }
                                    break;
                                }
                            }
                            try
                            {
                                if (!flag) // we didn't find the line
                                    throw new NotFoundException("Bus line not found !!");
                            }
                            catch (NotFoundException ex)
                            {
                                Console.WriteLine($" ERROR : {ex.ToString()}");
                            }
                        }
                        break;
                    case Choice.FIND:
                        Console.WriteLine("Enter your choice :");
                        Console.WriteLine("Find Lines for station - 1 / Find bus Line - 2");
                        innerChoice = getIntInput();
                        if (innerChoice == 1)
                        {
                            Console.WriteLine("Enter Bus station :");
                            int tmpNum = getIntInput();
                            try
                            {
                                List<Line> subList = myList.FindStation(tmpNum); // A new list of lines that pass through the station

                                foreach (Line item in subList)
                                {
                                    Console.WriteLine($"Line #{item.LineNumber}");
                                }
                            }
                            catch (NotFoundException ex) // wrong bus stop
                            {
                                Console.WriteLine($" ERROR : {ex.ToString()}");
                            }
                        }
                        if (innerChoice == 2) // Finding the fast route between two stations
                        {
                            Console.WriteLine("Enter first station :");
                            int stn1 = getIntInput();
                            Console.WriteLine("Enter second station :");
                            int stn2 = getIntInput();
                            // A new list of lines that pass between these 2 stations
                            BusLinesList subList = myList.CreateSubList(stn1, stn2);
                            try
                            {
                                // Sort the list according to a criterion of total travel time
                                subList.SortList();
                                foreach (Line item in subList)
                                {
                                    Console.WriteLine($"Line #{item.LineNumber}");
                                }
                            }
                            catch (NotFoundException ex) // wrong station/stations
                            {
                                Console.WriteLine($" ERROR : {ex.ToString()}");
                            }
                        }
                        break;
                    case Choice.PRINT:
                        Console.WriteLine("Enter your choice :");
                        Console.WriteLine("Print all lines - 1 / Print all stops - 2");
                        Console.WriteLine();
                        innerChoice = getIntInput();
                        if (innerChoice == 1) // Print all lines
                        {
                            foreach (Line item in myList)
                            {
                                Console.WriteLine($"Line #{item.LineNumber}");
                            }
                        }
                        if (innerChoice == 2) // Print all stops
                        {
                            List<int> stationList = new List<int> { }; // List of station numbers
                            foreach (Line line in myList) // For each line from the list of existing lines
                            {
                                foreach (BusLineStop stop in line)
                                {
                                    bool flag = true; // True if the bus stop is not yet in the list of printable bus stops
                                    foreach (int num in stationList) // For each station where the line stops
                                    {
                                        if (num == stop.BusStationKey) // We have already printed this station, it is already on the list
                                        {
                                            flag = false;
                                        }
                                    }
                                    if (flag == true) // the bus stop is not yet in the list of printable bus stops
                                    {
                                        List<Line> subList = myList.FindStation(stop.BusStationKey); // List of lines stopping at this station
                                        Console.WriteLine(stop.ToString()); // Prints the station
                                        foreach (Line item in subList) // Prints the lines that stop at the station
                                        {
                                            Console.Write($"Line #{item.LineNumber} / ");
                                        }
                                        Console.WriteLine(); // new line

                                        // Adds the station to the list of stations we have printed
                                        stationList.Add(stop.BusStationKey);
                                    }
                                }
                            }
                        }
                        break;
                    case Choice.EXIT:
                        Console.WriteLine("GoodBye");
                        break;
                    default:
                        break;
                }
            } while (myChoice != 5);
        }
        /// <summary>
        /// Gets input from user untill it is integer and returns it in integer form
        /// Throws exception if input is not an integer
        /// </summary>
        /// <returns>input casted into integer</returns>
        static public int getIntInput()
        {
            string tmpString;
            int tmpNum;
            do
            {
                tmpString = Console.ReadLine(); // reads the input
                try
                {
                    if (!int.TryParse(tmpString, out tmpNum)) // didn't succeed to switch the input to integer
                    {
                        throw new InvalidCastException("Invalid input !!");
                    }
                    break;
                }
                catch (InvalidCastException ex)
                {
                    Console.WriteLine($" ERROR : {ex.ToString()}");
                }
            } while (true);
            return tmpNum;
        }
        static Random r = new Random(DateTime.Now.Millisecond);
    }
}