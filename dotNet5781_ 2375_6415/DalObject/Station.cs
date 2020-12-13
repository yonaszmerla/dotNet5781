﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Device.Location;

namespace DalObject
{
    class Station
    {
        private int stationId;

        public int StationId
        {
            get { return stationId; }
            set { stationId = value; }
        }
        private GeoCoordinate coordinates;

        public  GeoCoordinate Coordinates
        {
            get { return coordinates; }
            set { coordinates = value; }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private bool invalid;

        public bool  Invalid
        {
            get { return invalid; }
            set { invalid = value; }
        }

        private bool roof;

        public bool Roof
        {
            get { return roof; }
            set { roof = value; }
        }

        private bool digitalPanel;

        public bool DigitalPanel
        {
            get { return digitalPanel; }
            set { digitalPanel = value; }
        }
    }
}