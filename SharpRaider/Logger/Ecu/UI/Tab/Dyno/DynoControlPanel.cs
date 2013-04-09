/*
 * This code is derived from the Java version of RomRaider
 *
 * RomRaider Open-Source Tuning, Logging and Reflashing
 * Copyright (C) 2006-2012 RomRaider.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with this program; if not, write to the Free Software Foundation, Inc.,
 * 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
 */

using System;
using System.Collections.Generic;
using System.IO;
using Com.Centerkey.Utils;
using Java.Awt;
using Java.Awt.Event;
using Javax.Swing;
using Javax.Swing.Border;
using Javax.Swing.Text;
using Javax.Xml.Parsers;
using Org.W3c.Dom;
using Org.Xml.Sax;
using RomRaider;
using RomRaider.Editor.Ecu;
using RomRaider.Logger.Car.Util;
using RomRaider.Logger.Ecu.Definition;
using RomRaider.Logger.Ecu.UI;
using RomRaider.Logger.Ecu.UI.Tab.Dyno;
using RomRaider.Util;
using Sharpen;

namespace RomRaider.Logger.Ecu.UI.Tab.Dyno
{
	[System.Serializable]
	public sealed class DynoControlPanel : JPanel
	{
		private const long serialVersionUID = 3787020251963102201L;

		private static readonly Org.Apache.Log4j.Logger LOGGER = Org.Apache.Log4j.Logger.
			GetLogger(typeof(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel));

		private static readonly string CARS_FILE = ECUExec.settings.GetCarsDefFilePath();

		private static readonly string MISSING_CAR_DEF = "Missing cars_def.xml";

		private static readonly string ENGINE_SPEED = "P8";

		private static readonly string VEHICLE_SPEED = "P9";

		private static readonly string IAT = "P11";

		private static readonly string THROTTLE_ANGLE = "P13";

		private static readonly string ATM = "P24";

		private static readonly string MANUAL = "manual";

		private static readonly string IMPERIAL = "Imperial";

		private static readonly string METRIC = "Metric";

		private static readonly string DYNO_MODE = "Dyno";

		private static readonly string ET_MODE = "ET";

		private static readonly string CAR_MASS_TT = "Base mass of car from factory";

		private static readonly string DELTA_MASS_TT = "Mass of all occupants and accessories added";

		private static readonly string HUMIDITY_TT = "Current relative Humidity";

		private static readonly string TIRE_WIDTH_TT = "Tire width in millimeters";

		private static readonly string TIRE_ASPECT_TT = "Tire aspect ratio in percentage";

		private static readonly string WHEEL_SIZE_TT = "Wheel (rim) size in inches";

		private static readonly string CAR_SELECT_TT = "Select car, default is first in list";

		private static readonly string GEAR_SELECT_TT = "Select gear, default is 2nd for 4AT, 3rd for 5MT and 4th for 6MT";

		private static readonly string RPM_MIN_TT = "RPM min is updated after WOT";

		private static readonly string RPM_MAX_TT = "RPM max is updated after WOT";

		private static readonly string ELEVATION_TT = "Elevation is calculated from ECU ATM sensor";

		private static readonly string AMB_TEMP_TT = "Ambient Temperature is updated from IAT sensor";

		private static readonly string ORDER_TT = "Lower number provides more smoothing";

		private static readonly string RESET_TT = "This clears all recorded or file loaded data";

		private static readonly string RECORD_TT = "Press to acquire data, multiple sets of Dyno data can be acquired";

		private static readonly string DYNO_TT = "Use this mode to estimate Power & Torque";

		private static readonly string ET_TT = "Use this mode to measure trap times";

		private static readonly string COLON = ":";

		private static readonly string COMMA = ",";

		private static readonly string SEMICOLON = ";";

		private static readonly string TAB = "\u0009";

		private static readonly string RR_LOG_TIME = "Time";

		private static readonly string COBB_AP_TIME = "Seconds";

		private static readonly string COBB_ATR_TIME = "Time Stamp";

		private static readonly string AEM_LOG_TIME = "Time/s";

		private static readonly string OP2_LOG_TIME = "time";

		private static readonly string LOG_RPM = "RPM";

		private static readonly string LOG_ES = "Engine Speed";

		private static readonly string LOG_TA = "Throttle";

		private static readonly string LOG_VS = "Vehicle Speed";

		private static readonly string LOG_VS_I = "mph";

		private static readonly string LOG_VS_M = "km/h";

		private const double KPH_2_MPH = 1.609344;

		private readonly DataRegistrationBroker broker;

		private readonly DynoChartPanel chartPanel;

		private readonly Component parent;

		private static FocusAdapter allTextSelector;

		private IList<ExternalData> externals = new AList<ExternalData>();

		private IList<EcuParameter> @params = new AList<EcuParameter>();

		private IList<EcuSwitch> switches = new AList<EcuSwitch>();

		private string[] gearList;

		private double tSize;

		private double rpm2mph;

		private double mass;

		private double altitude;

		private double humidity;

		private double pressure;

		private double airTemp;

		private double pSat;

		private double P_v;

		private double P_d;

		private double airDen;

		private double fToE;

		private double sToE;

		private double tToS;

		private double reFfToE;

		private double reFsToE;

		private double reFtToS;

		private double reFauc;

		private double auc;

		private double aucStart;

		private long fTime;

		private long sTime;

		private long eTime;

		private long ttTime;

		private long stTime;

		private bool getEnv;

		private bool wotSet;

		private string path;

		private string carInfo;

		private string[] carTypeArr;

		private string[] carMassArr;

		private string[] dragCoeffArr;

		private string[] rollCoeffArr;

		private string[] frontalAreaArr;

		private string[] gearRatioArr;

		private IList<string[]> gearsRatioArr;

		private string[] finalRatioArr;

		private string[] transArr;

		private string[] widthArr;

		private string[] aspectArr;

		private string[] sizeArr;

		private readonly JTextField carMass = new JTextField("0", 4);

		private readonly JTextField deltaMass = new JTextField("225", 4);

		private readonly JTextField dragCoeff = new JTextField("0", 4);

		private readonly JTextField rollCoeff = new JTextField("0", 4);

		private readonly JTextField frontalArea = new JTextField("0", 4);

		private readonly JTextField rpmMin = new JTextField("2000", 4);

		private readonly JTextField rpmMax = new JTextField("6500", 4);

		private readonly JTextField elevation = new JTextField("200", 4);

		private readonly JTextField relHumid = new JTextField("60", 4);

		private readonly JTextField ambTemp = new JTextField("68", 4);

		private readonly JTextField gearRatio = new JTextField("0", 4);

		private readonly JTextField finalRatio = new JTextField("0", 4);

		private readonly JTextField transmission = new JTextField("0", 4);

		private readonly JTextField tireWidth = new JTextField("0", 4);

		private readonly JTextField tireAspect = new JTextField("0", 4);

		private readonly JTextField tireSize = new JTextField("0", 4);

		private readonly JLabel elevLabel = new JLabel("Elevation (ft)");

		private readonly JLabel tempLabel = new JLabel("Air Temperature (\u00b0F)");

		private readonly JLabel deltaMassLabel = new JLabel("Delta Weight (lbs)");

		private readonly JLabel carMassLabel = new JLabel("Base Weight (lbs)");

		private string units = IMPERIAL;

		private string preUnits = IMPERIAL;

		private string elevUnits = "ft";

		private string tempUnits = "\u00b0F";

		private double atm;

		private string pressUnits = "psi";

		private string pressText = string.Format("%1.2f", 14.7);

		private string iatLogUnits = "F";

		private string atmLogUnits = "psi";

		private string vsLogUnits = LOG_VS_I;

		private readonly double[] results = new double[5];

		private readonly string[] resultStrings = new string[6];

		private double distance;

		private long lastET;

		private double[] etResults = new double[12];

		private readonly JPanel filterPanel = new JPanel();

		private readonly JPanel unitsPanel = new JPanel();

		private readonly JPanel iPanel = new JPanel();

		private readonly JPanel refPanel = new JPanel();

		private readonly JPanel etPanel = new JPanel();

		private readonly JComboBox orderComboBox;

		private readonly JComboBox carSelectBox;

		private readonly JComboBox gearSelectBox;

		private readonly JButton interpolateButton = new JButton("Recalculate");

		private readonly JToggleButton recordDataButton = new JToggleButton("Record Data"
			);

		private readonly JToggleButton recordButton;

		private readonly JRadioButton dButton = new JRadioButton(DYNO_MODE);

		private readonly JRadioButton eButton = new JRadioButton(ET_MODE);

		private readonly JRadioButton iButton = new JRadioButton(IMPERIAL);

		private readonly JRadioButton mButton = new JRadioButton(METRIC);

		private readonly JCheckBox loadFileCB = new JCheckBox("Load From File");

		private Settings settings;

		public DynoControlPanel(Component parent, DataRegistrationBroker broker, ECUEditor
			 ecuEditor, DynoChartPanel chartPanel)
		{
			orderComboBox = BuildPolyOrderComboBox();
			carSelectBox = BuildCarSelectComboBox();
			gearSelectBox = BuildGearComboBox();
			recordButton = BuildRecordDataButton();
			//    private static final String SI = "SI";
			//    private String hpUnits = "hp(I)";
			//    private String tqUnits = "lbf-ft";
			ParamChecker.CheckNotNull(parent, broker, chartPanel);
			settings = ECUExec.settings;
			this.parent = parent;
			this.broker = broker;
			this.chartPanel = chartPanel;
			AddControls();
		}

		private void CalculateEnv()
		{
			if (units.Equals(IMPERIAL))
			{
				altitude = ParseDouble(elevation) * 0.3048;
				// feet to meters
				airTemp = (ParseDouble(ambTemp) + 459.67) * 5 / 9;
				//[K] = ([F] + 459.67) * 5/9
				mass = (ParseDouble(carMass) + ParseDouble(deltaMass)) * 0.4536;
				//lbs to kg
				pressure = atm * 6894.75728;
			}
			// [Pa] = [psi] * 6894.75728
			if (units.Equals(METRIC))
			{
				altitude = ParseDouble(elevation);
				// meters
				airTemp = ParseDouble(ambTemp) + 273.15;
				//[K] = [C] + 273.15
				mass = (ParseDouble(carMass) + ParseDouble(deltaMass));
				//kg
				pressure = atm * 1000;
			}
			// [Pa] = [kPa] * 1000
			//        if (units.equals(SI)) {
			//            altitude = parseDouble(elevation);    // meters
			//            airTemp = parseDouble(ambTemp);    //[K]
			//            mass = (parseDouble(carMass) + parseDouble(deltaMass));    //kg
			//        }
			tSize = ParseDouble(tireSize) + ParseDouble(tireWidth) / 25.4 * ParseDouble(tireAspect
				) / 100 * 2;
			rpm2mph = ParseDouble(gearRatio) * ParseDouble(finalRatio) / (tSize * 0.002975);
			humidity = ParseDouble(relHumid) / 100;
			//        carInfo = (String) carSelectBox.getSelectedItem() + "(" + gearSelectBox.getSelectedItem() + "), Pres: " + pressText +
			//          pressUnits + ", Hum: " + relHumid.getText().trim() + "%, Temp: " + ambTemp.getText().trim() + tempUnits;
			// Use elevation if ATM was not read from ECU
			if (atm == 0)
			{
				pressure = 101325 * Math.Pow((1 - 22.5577 * Math.Pow(10, -6) * altitude), 5.25578
					);
			}
			//Pressure at altitude [Pa]
			pSat = 610.78 * Math.Pow(10, ((7.5 * airTemp - 2048.625) / (airTemp - 35.85)));
			//Saturation vapor pressure [Pa]
			P_v = humidity * pSat;
			P_d = pressure - P_v;
			airDen = P_d / (287.05 * airTemp) + P_v / (461.495 * airTemp);
			//air density with humidity included [kg/m^3]
			carInfo = carSelectBox.GetSelectedItem() + "(" + gearSelectBox.GetSelectedItem() 
				+ "); Elev: " + elevation.GetText().Trim() + elevUnits + "; Pres: " + pressText 
				+ pressUnits + "; Hum: " + relHumid.GetText().Trim() + "%; Temp: " + ambTemp.GetText
				().Trim() + tempUnits;
		}

		private double CalcHp(double rpm, double accel, long now)
		{
			double mph = SpeedCalculator.CalculateMph(rpm, rpm2mph);
			double accelG = accel * 45.5486542443;
			// calculate Drive power = power required to propel vehicle mass at certain speed and acceleration.
			// =Force*Velocity=Mass*Accel*Velocity
			// Accel(m/s/s)*Mass(kg)*Velocity(m/s)
			double dP = 9.8067 * accelG * mass * 0.44704 * mph;
			// calculate Roll HP = Power required to counter rolling resistance
			// Velocity(m/s)*Friction force
			double rP = 0.44704 * mph * ParseDouble(rollCoeff) * mass * 9.8067;
			// calculate Wind HP = Power required to counter wind drag
			// Aero drag
			double aP = 0.5 * ParseDouble(dragCoeff) * airDen * 0.0929 * ParseDouble(frontalArea
				) * Math.Pow(0.44704 * mph, 3);
			double hp = dP + rP + aP;
			if (units.Equals(IMPERIAL))
			{
				hp = hp / 745.7;
			}
			if (units.Equals(METRIC))
			{
				hp = hp / 1000;
			}
			// Calculate acceleration statistics
			fTime = (mph <= 50) ? now : fTime;
			sTime = (mph <= 60) ? now : sTime;
			eTime = (mph <= 80) ? now : eTime;
			ttTime = (rpm <= 3000) ? now : ttTime;
			stTime = (rpm <= 6000) ? now : stTime;
			fToE = (eTime - fTime) / 1000.0;
			// 50-80mph, sec
			sToE = (eTime - sTime) / 1000.0;
			// 60-80mph, sec
			tToS = (stTime - ttTime) / 1000.0;
			// 3-6k rpm, sec
			return hp;
		}

		public double CalcRpm(double vs)
		{
			return SpeedCalculator.CalculateRpm(vs, rpm2mph, vsLogUnits);
		}

		public void UpdateEnv(double iat, double at_press)
		{
			getEnv = false;
			DeregisterData(IAT, ATM);
			if (units.Equals(IMPERIAL))
			{
				if (iatLogUnits.Equals("C"))
				{
					iat = (iat * 9 / 5) + 32;
				}
				if (atmLogUnits.Equals("bar"))
				{
					at_press = at_press * 14.503773801;
				}
				if (at_press > 0)
				{
					altitude = 145442 * (1 - Math.Pow(((at_press / (1.45 * Math.Pow(10, -2))) / 1013.25
						), 0.190263));
				}
			}
			// Altitude in ft from ATM in psi
			if (units.Equals(METRIC))
			{
				if (iatLogUnits.Equals("F"))
				{
					iat = (iat - 32) * 5 / 9;
				}
				if (atmLogUnits.Equals("bar"))
				{
					if (at_press > 0)
					{
						altitude = 145442 * (1 - Math.Pow((((at_press * 14.503773801) / (1.45 * Math.Pow(
							10, -2))) / 1013.25), 0.190263)) * 0.3048;
					}
					// Altitude in m from ATM in psi
					at_press = at_press * 100;
				}
				if (atmLogUnits.Equals("psi"))
				{
					if (at_press > 0)
					{
						altitude = 145442 * (1 - Math.Pow(((at_press / (1.45 * Math.Pow(10, -2))) / 1013.25
							), 0.190263)) * 0.3048;
					}
					// Altitude in m from ATM in psi
					at_press = at_press * 6.89475728;
				}
			}
			atm = at_press;
			ambTemp.SetText(string.Format("%1.1f", iat));
			pressText = string.Format("%1.3f", atm);
			if (atm > 0)
			{
				elevation.SetText(string.Format("%1.0f", altitude));
			}
			// disable user input if ECU parameters recorded
			//        ambTemp.setEnabled(false);
			elevation.SetEnabled(false);
			CalculateEnv();
			UpdateChart();
		}

		private void UpdateChart()
		{
			chartPanel.QuietUpdate(false);
			auc = 0;
			aucStart = 0;
			double maxHp = 0;
			double maxHpRpm = 0;
			double maxTq = 0;
			double maxTqRpm = 0;
			double nowHp = 0;
			double nowTq = 0;
			int order = (int)orderComboBox.GetSelectedItem();
			double[] speedArray = Arrays.CopyOf(chartPanel.GetRpmCoeff(order), chartPanel.GetRpmCoeff
				(order).Length);
			LOGGER.Info("DYNO Speed Coeffecients: " + Arrays.ToString(speedArray));
			double[] accelArray = new double[(order)];
			for (int x = 0; x < order; x++)
			{
				accelArray[x] = (order - x) * speedArray[x];
			}
			LOGGER.Info("DYNO Accel Coeffecients: " + Arrays.ToString(accelArray));
			int samples = chartPanel.GetSampleCount();
			LOGGER.Info("DYNO Sample Count: " + samples);
			double timeMin = chartPanel.GetTimeSample(0);
			double timeMax = chartPanel.GetTimeSample(samples - 1);
			for (double x_1 = timeMin; x_1 <= timeMax; x_1 = x_1 + 10)
			{
				double speedSample = 0;
				double accelSample = 0;
				// Calculate smoothed SPEED from coefficients
				for (int i = 0; i <= order; i++)
				{
					int pwr = order - i;
					speedSample = speedSample + (Math.Pow(x_1, pwr) * speedArray[i]);
				}
				// Calculate acceleration from first derivative of SPEED coefficients
				for (int i_1 = 0; i_1 < order; i_1++)
				{
					int pwr = order - i_1 - 1;
					accelSample = accelSample + (Math.Pow(x_1, pwr) * accelArray[i_1]);
				}
				if (IsManual())
				{
					accelSample = accelSample / rpm2mph;
				}
				else
				{
					// RPM acceleration from RPM
					speedSample = SpeedCalculator.CalculateRpm(speedSample, rpm2mph, vsLogUnits);
					// convert logged vs to RPM for AT
					if (vsLogUnits.Equals(LOG_VS_M))
					{
						accelSample = accelSample / KPH_2_MPH;
					}
				}
				if (CheckInRange("RPM", rpmMin, rpmMax, speedSample))
				{
					nowHp = CalcHp(speedSample, accelSample, (long)x_1);
					nowTq = TorqueCalculator.CalculateTorque(speedSample, nowHp, units);
					chartPanel.AddData(speedSample, nowHp, nowTq);
					if (nowHp > maxHp)
					{
						maxHp = nowHp;
						maxHpRpm = speedSample;
					}
					if (nowTq > maxTq)
					{
						maxTq = nowTq;
						maxTqRpm = speedSample;
					}
					if (speedSample >= 3000 && speedSample <= 6000)
					{
						if (aucStart == 0)
						{
							aucStart = (nowHp * speedSample) + (nowTq * speedSample);
						}
						auc = auc + Math.Abs((nowHp * speedSample) + (nowTq * speedSample) - aucStart);
					}
				}
			}
			chartPanel.QuietUpdate(true);
			auc = auc / 1e6 / tToS;
			results[0] = maxHp;
			results[1] = maxHpRpm;
			results[2] = maxTq;
			results[3] = maxTqRpm;
			string hpUnits = " hp(I)";
			string tqUnits = " lbf-ft";
			resultStrings[2] = "50-80 MPH: " + string.Format("%1.2f", fToE) + " secs";
			resultStrings[3] = "60-80 MPH: " + string.Format("%1.2f", sToE) + " secs";
			if (units.Equals(METRIC))
			{
				hpUnits = " kW";
				tqUnits = " N-m";
				resultStrings[2] = "80-130 km/h: " + string.Format("%1.2f", fToE) + " secs";
				resultStrings[3] = "100-130 km/h: " + string.Format("%1.2f", sToE) + " secs";
			}
			resultStrings[0] = carInfo;
			resultStrings[1] = "Max Pwr: " + string.Format("%1.1f", maxHp) + hpUnits + " @ " 
				+ string.Format("%1.0f", maxHpRpm) + " RPM / Max TQ: " + string.Format("%1.1f", 
				maxTq) + tqUnits + " @ " + string.Format("%1.0f", maxTqRpm) + " RPM";
			resultStrings[4] = "3000-6000 RPM: " + string.Format("%1.2f", tToS) + " secs";
			resultStrings[5] = "3000-6000 RPM: " + string.Format("%1.2f", auc) + " AUC";
			if (reFfToE > 0)
			{
				resultStrings[2] = resultStrings[2] + " (" + string.Format("%1.2f", (fToE - reFfToE
					)) + ")";
			}
			if (reFsToE > 0)
			{
				resultStrings[3] = resultStrings[3] + " (" + string.Format("%1.2f", (sToE - reFsToE
					)) + ")";
			}
			if (reFtToS > 0)
			{
				resultStrings[4] = resultStrings[4] + " (" + string.Format("%1.2f", (tToS - reFtToS
					)) + ")";
			}
			if (reFauc > 0)
			{
				resultStrings[5] = resultStrings[5] + " (" + string.Format("%1.2f", (auc - reFauc
					)) + ")";
			}
			LOGGER.Info("DYNO Results: " + carInfo);
			LOGGER.Info("DYNO Results: " + resultStrings[1]);
			LOGGER.Info("DYNO Results: " + resultStrings[2]);
			LOGGER.Info("DYNO Results: " + resultStrings[3]);
			LOGGER.Info("DYNO Results: " + resultStrings[4]);
			LOGGER.Info("DYNO Results: " + resultStrings[5]);
			chartPanel.Interpolate(results, resultStrings);
			parent.Repaint();
		}

		private void UpdateET()
		{
			chartPanel.QuietUpdate(false);
			int order = 5;
			double x1 = 0;
			distance = 0;
			lastET = 0;
			double[] speedArray = Arrays.CopyOf(chartPanel.GetRpmCoeff(order), chartPanel.GetRpmCoeff
				(order).Length);
			LOGGER.Info("DYNO Speed Coeffecients: " + Arrays.ToString(speedArray));
			int samples = chartPanel.GetSampleCount();
			LOGGER.Info("DYNO Sample Count: " + samples);
			double timeMin = chartPanel.GetTimeSample(0);
			double timeMax = chartPanel.GetTimeSample(samples - 1);
			for (double x = timeMin; x <= timeMax; x = x + 1)
			{
				double speedSample = 0;
				// Calculate smoothed SPEED from coefficients
				for (int i = 0; i <= order; i++)
				{
					int pwr = order - i;
					speedSample = speedSample + (Math.Pow(x, pwr) * speedArray[i]);
				}
				chartPanel.AddData((x / 1000), speedSample);
				if (vsLogUnits.Equals(LOG_VS_M))
				{
					speedSample = (speedSample / KPH_2_MPH);
				}
				distance = distance + (speedSample * 5280 / 3600 * (x - lastET) / 1000);
				lastET = (long)x;
				x1 = x / 1000;
				if (distance <= 60)
				{
					etResults[0] = x1;
				}
				if (distance <= 60)
				{
					etResults[1] = speedSample;
				}
				if (distance <= 330)
				{
					etResults[2] = x1;
				}
				if (distance <= 330)
				{
					etResults[3] = speedSample;
				}
				if (distance <= 660)
				{
					etResults[4] = x1;
				}
				if (distance <= 660)
				{
					etResults[5] = speedSample;
				}
				if (distance <= 1000)
				{
					etResults[6] = x1;
				}
				if (distance <= 1000)
				{
					etResults[7] = speedSample;
				}
				if (distance <= 1320)
				{
					etResults[8] = x1;
				}
				if (distance <= 1320)
				{
					etResults[9] = speedSample;
				}
				if (speedSample <= 60)
				{
					etResults[10] = x1;
				}
				if (speedSample <= 60)
				{
					etResults[11] = speedSample;
				}
			}
			if (vsLogUnits.Equals(LOG_VS_M))
			{
				etResults[1] = etResults[1] * KPH_2_MPH;
				etResults[3] = etResults[3] * KPH_2_MPH;
				etResults[5] = etResults[5] * KPH_2_MPH;
				etResults[7] = etResults[7] * KPH_2_MPH;
				etResults[9] = etResults[9] * KPH_2_MPH;
				etResults[11] = etResults[11] * KPH_2_MPH;
			}
			chartPanel.QuietUpdate(true);
			LOGGER.Info("ET Split 60: " + string.Format("%1.3f", etResults[0]));
			LOGGER.Info("ET Split 330: " + string.Format("%1.3f", etResults[2]));
			LOGGER.Info("ET Split 1/8: " + string.Format("%1.3f", etResults[4]) + " @ " + string
				.Format("%1.2f", etResults[5]));
			LOGGER.Info("ET Split 1000: " + string.Format("%1.3f", etResults[6]));
			LOGGER.Info("ET Split 1/4: " + string.Format("%1.3f", etResults[8]) + " @ " + string
				.Format("%1.2f", etResults[9]));
			LOGGER.Info("ET 0 to " + string.Format("%1.0f", etResults[11]) + " " + vsLogUnits
				 + ": " + string.Format("%1.3f", etResults[10]));
			chartPanel.UpdateEtResults(carInfo, etResults, vsLogUnits);
			parent.Repaint();
		}

		public bool IsValidET(long now, double vs)
		{
			try
			{
				//            LOGGER.trace("lastET: " + lastET + " now: " + now + " VS: " + vs);
				if (vs > 0)
				{
					if (vsLogUnits.Equals(LOG_VS_M))
					{
						vs = (vs / KPH_2_MPH);
					}
					distance = distance + (vs * 5280 / 3600 * (now - lastET) / 1000);
					LOGGER.Info("ET Distance (ft): " + distance);
					if (distance > 1330)
					{
						recordDataButton.SetSelected(false);
						DeregisterData(VEHICLE_SPEED);
						chartPanel.ClearPrompt();
						UpdateET();
						return false;
					}
					return true;
				}
				return false;
			}
			finally
			{
				lastET = now;
			}
		}

		public bool IsValidData(double rpm, double ta)
		{
			if (wotSet && (ta < 99))
			{
				recordDataButton.SetSelected(false);
				rpmMax.SetText(string.Format("%1.0f", rpm));
				Deregister();
			}
			else
			{
				if (ta > 98)
				{
					if (!wotSet)
					{
						rpmMin.SetText(string.Format("%1.0f", rpm));
					}
					wotSet = true;
					return true;
				}
			}
			wotSet = false;
			return false;
		}

		public bool IsManual()
		{
			return transmission.GetText().Trim().Equals(MANUAL);
		}

		private void Deregister()
		{
			if (IsManual())
			{
				DeregisterData(ENGINE_SPEED, THROTTLE_ANGLE);
			}
			else
			{
				DeregisterData(VEHICLE_SPEED, THROTTLE_ANGLE);
			}
			RegisterData(IAT, ATM);
			getEnv = true;
		}

		public bool GetEnv()
		{
			return getEnv;
		}

		private void AddControls()
		{
			JPanel panel = new JPanel();
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			Add(panel, gridBagLayout, BuildModePanel(), 0, 0, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildFilterPanel(), 0, 1, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildRadioPanel(), 0, 2, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildInterpolatePanel(), 0, 3, 1, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, BuildReferencePanel(), 0, 4, 1, GridBagConstraints.HORIZONTAL
				);
			//        add(panel, gridBagLayout, buildEtPanel(), 0, 5, 1, HORIZONTAL);
			Add(panel);
		}

		private void Add(JPanel panel, GridBagLayout gridBagLayout, JComponent component, 
			int x, int y, int spanX, int fillType)
		{
			GridBagConstraints constraints = BuildBaseConstraints();
			UpdateConstraints(constraints, x, y, spanX, 1, 1, 1, fillType);
			gridBagLayout.SetConstraints(component, constraints);
			panel.Add(component);
		}

		private JPanel BuildRadioPanel()
		{
			//        JPanel panel = new JPanel();
			unitsPanel.SetBorder(new TitledBorder("Measurement Units"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			unitsPanel.SetLayout(gridBagLayout);
			BuildRadioButtons(unitsPanel);
			return unitsPanel;
		}

		private JPanel BuildModePanel()
		{
			JPanel panel = new JPanel();
			panel.SetBorder(new TitledBorder("Mode"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			panel.SetLayout(gridBagLayout);
			BuildModeButtons(panel);
			return panel;
		}

		private JPanel BuildInterpolatePanel()
		{
			iPanel.SetBorder(new TitledBorder("Recalculate"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			iPanel.SetLayout(gridBagLayout);
			AddLabeledComponent(iPanel, gridBagLayout, "Smoothing Factor", orderComboBox, 0);
			AddComponent(iPanel, gridBagLayout, BuildInterpolateButton(orderComboBox), 2);
			AddMinMaxFilter(iPanel, gridBagLayout, "RPM Range", rpmMin, rpmMax, 4);
			Add(iPanel, gridBagLayout, elevLabel, 0, 6, 3, GridBagConstraints.HORIZONTAL);
			Add(iPanel, gridBagLayout, elevation, 1, 7, 0, GridBagConstraints.NONE);
			Add(iPanel, gridBagLayout, tempLabel, 0, 8, 3, GridBagConstraints.HORIZONTAL);
			Add(iPanel, gridBagLayout, ambTemp, 1, 9, 0, GridBagConstraints.NONE);
			AddLabeledComponent(iPanel, gridBagLayout, "Rel Humidity (%)", relHumid, 10);
			SetSelectAllFieldText(rpmMin);
			SetSelectAllFieldText(rpmMax);
			SetSelectAllFieldText(elevation);
			SetSelectAllFieldText(ambTemp);
			SetSelectAllFieldText(relHumid);
			return iPanel;
		}

		private JPanel BuildReferencePanel()
		{
			refPanel.SetBorder(new TitledBorder("Reference Trace"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			refPanel.SetLayout(gridBagLayout);
			Add(refPanel, gridBagLayout, BuildOpenReferenceButton(), 0, 0, 1, GridBagConstraints
				.NONE);
			Add(refPanel, gridBagLayout, BuildSaveReferenceButton(), 1, 0, 1, GridBagConstraints
				.NONE);
			Add(refPanel, gridBagLayout, BuildClearReferenceButton(), 2, 0, 1, GridBagConstraints
				.NONE);
			return refPanel;
		}

		private JPanel BuildEtPanel()
		{
			etPanel.SetBorder(new TitledBorder("Elapsed Time"));
			etPanel.SetVisible(false);
			GridBagLayout gridBagLayout = new GridBagLayout();
			etPanel.SetLayout(gridBagLayout);
			AddLabeledComponent(etPanel, gridBagLayout, "Select Car", carSelectBox, 0);
			AddComponent(etPanel, gridBagLayout, recordButton, 2);
			Add(etPanel, gridBagLayout, BuildSaveReferenceButton(), 1, 3, 1, GridBagConstraints
				.NONE);
			return etPanel;
		}

		private void AddLabeledComponent(JPanel panel, GridBagLayout gridBagLayout, string
			 name, JComponent component, int y)
		{
			Add(panel, gridBagLayout, new JLabel(name), 0, y, 3, GridBagConstraints.HORIZONTAL
				);
			Add(panel, gridBagLayout, component, 0, y + 1, 3, GridBagConstraints.NONE);
		}

		private JPanel BuildFilterPanel()
		{
			ChangeCars(0);
			SetToolTips();
			filterPanel.SetBorder(new TitledBorder("Dyno Settings"));
			GridBagLayout gridBagLayout = new GridBagLayout();
			filterPanel.SetLayout(gridBagLayout);
			Add(filterPanel, gridBagLayout, new JLabel("Wheel (Width/Aspect-Diam.)"), 0, 15, 
				3, GridBagConstraints.HORIZONTAL);
			Add(filterPanel, gridBagLayout, tireWidth, 0, 16, 1, GridBagConstraints.NONE);
			Add(filterPanel, gridBagLayout, tireAspect, 1, 16, 1, GridBagConstraints.NONE);
			Add(filterPanel, gridBagLayout, tireSize, 2, 16, 1, GridBagConstraints.NONE);
			AddLabeledComponent(filterPanel, gridBagLayout, "Select Car", carSelectBox, 18);
			AddLabeledComponent(filterPanel, gridBagLayout, "Select Gear", gearSelectBox, 21);
			Add(filterPanel, gridBagLayout, deltaMassLabel, 0, 24, 3, GridBagConstraints.HORIZONTAL
				);
			Add(filterPanel, gridBagLayout, deltaMass, 1, 25, 1, GridBagConstraints.NONE);
			Add(filterPanel, gridBagLayout, carMassLabel, 0, 27, 3, GridBagConstraints.HORIZONTAL
				);
			Add(filterPanel, gridBagLayout, carMass, 1, 28, 1, GridBagConstraints.NONE);
			AddComponent(filterPanel, gridBagLayout, recordButton, 31);
			AddComponent(filterPanel, gridBagLayout, BuildLoadFileCB(), 32);
			AddComponent(filterPanel, gridBagLayout, BuildResetButton(), 33);
			//        addLabeledComponent(panel, gridBagLayout, "Drag Coeff", dragCoeff, 33);
			//        addLabeledComponent(panel, gridBagLayout, "Frontal Area", frontalArea, 36);
			//        addLabeledComponent(panel, gridBagLayout, "Rolling Resist Coeff", rollCoeff, 39);
			SetSelectAllFieldText(tireWidth);
			SetSelectAllFieldText(tireAspect);
			SetSelectAllFieldText(tireSize);
			SetSelectAllFieldText(deltaMass);
			SetSelectAllFieldText(carMass);
			return filterPanel;
		}

		private void SetToolTips()
		{
			relHumid.SetToolTipText(HUMIDITY_TT);
			carMass.SetToolTipText(CAR_MASS_TT);
			deltaMass.SetToolTipText(DELTA_MASS_TT);
			tireWidth.SetToolTipText(TIRE_WIDTH_TT);
			tireAspect.SetToolTipText(TIRE_ASPECT_TT);
			tireSize.SetToolTipText(WHEEL_SIZE_TT);
			carSelectBox.SetToolTipText(CAR_SELECT_TT);
			gearSelectBox.SetToolTipText(GEAR_SELECT_TT);
			rpmMin.SetToolTipText(RPM_MIN_TT);
			rpmMax.SetToolTipText(RPM_MAX_TT);
			elevation.SetToolTipText(ELEVATION_TT);
			ambTemp.SetToolTipText(AMB_TEMP_TT);
			orderComboBox.SetToolTipText(ORDER_TT);
			recordDataButton.SetToolTipText(RECORD_TT);
			dButton.SetToolTipText(DYNO_TT);
			eButton.SetToolTipText(ET_TT);
		}

		private JButton BuildResetButton()
		{
			JButton resetButton = new JButton("Clear Data");
			resetButton.AddActionListener(new _ActionListener_718(this));
			resetButton.SetToolTipText(RESET_TT);
			return resetButton;
		}

		private sealed class _ActionListener_718 : ActionListener
		{
			public _ActionListener_718(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.Clear();
				this._enclosing.parent.Repaint();
			}

			private readonly DynoControlPanel _enclosing;
		}

		private JToggleButton BuildRecordDataButton()
		{
			if (!carTypeArr[0].Trim().Equals(MISSING_CAR_DEF))
			{
				recordDataButton.AddActionListener(new _ActionListener_730(this));
			}
			else
			{
				recordDataButton.SetText(MISSING_CAR_DEF);
			}
			return recordDataButton;
		}

		private sealed class _ActionListener_730 : ActionListener
		{
			public _ActionListener_730(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.elevation.SetEnabled(true);
				if (this._enclosing.dButton.IsSelected())
				{
					if (this._enclosing.loadFileCB.IsSelected())
					{
						this._enclosing.LoadFromFile();
					}
					else
					{
						if (this._enclosing.recordDataButton.IsSelected())
						{
							this._enclosing.chartPanel.ClearGraph();
							this._enclosing.parent.Repaint();
							this._enclosing.CalculateEnv();
							if (this._enclosing.IsManual())
							{
								this._enclosing.RegisterData(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.ENGINE_SPEED
									, RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.THROTTLE_ANGLE);
							}
							else
							{
								this._enclosing.RegisterData(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.VEHICLE_SPEED
									, RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.THROTTLE_ANGLE);
							}
							this._enclosing.chartPanel.StartPrompt("wot");
						}
						else
						{
							this._enclosing.recordDataButton.SetSelected(false);
							if (this._enclosing.IsManual())
							{
								this._enclosing.DeregisterData(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.
									ENGINE_SPEED, RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.THROTTLE_ANGLE);
							}
							else
							{
								this._enclosing.DeregisterData(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.
									VEHICLE_SPEED, RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.THROTTLE_ANGLE);
							}
							this._enclosing.chartPanel.ClearPrompt();
						}
					}
				}
				if (this._enclosing.eButton.IsSelected())
				{
					if (this._enclosing.recordDataButton.IsSelected())
					{
						this._enclosing.chartPanel.Clear();
						this._enclosing.parent.Repaint();
						this._enclosing.CalculateEnv();
						this._enclosing.RegisterData(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.VEHICLE_SPEED
							);
						this._enclosing.chartPanel.StartPrompt(this._enclosing.vsLogUnits);
						this._enclosing.distance = 0;
						this._enclosing.lastET = 0;
					}
					else
					{
						this._enclosing.DeregisterData(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.
							VEHICLE_SPEED);
						this._enclosing.recordDataButton.SetSelected(false);
						this._enclosing.chartPanel.ClearPrompt();
					}
				}
			}

			private readonly DynoControlPanel _enclosing;
		}

		private JCheckBox BuildLoadFileCB()
		{
			loadFileCB.AddActionListener(new _ActionListener_782(this));
			return loadFileCB;
		}

		private sealed class _ActionListener_782 : ActionListener
		{
			public _ActionListener_782(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				if (this._enclosing.loadFileCB.IsSelected())
				{
					this._enclosing.recordDataButton.SetText("Read From File");
				}
				else
				{
					this._enclosing.recordDataButton.SetText("Record Data");
				}
			}

			private readonly DynoControlPanel _enclosing;
		}

		public bool IsRecordData()
		{
			return recordDataButton.IsSelected();
		}

		public bool IsRecordET()
		{
			return recordDataButton.IsSelected() && eButton.IsSelected();
		}

		private void BuildModeButtons(JPanel panel)
		{
			dButton.AddActionListener(new _ActionListener_804(this));
			//                etPanel.setVisible(false);
			//                filterPanel.setVisible(true);
			//                refPanel.setVisible(true);
			dButton.SetSelected(true);
			eButton.AddActionListener(new _ActionListener_822(this));
			//                filterPanel.setVisible(false);
			//                refPanel.setVisible(false);
			//                etPanel.setVisible(true);
			ButtonGroup group = new ButtonGroup();
			group.Add(dButton);
			group.Add(eButton);
			panel.Add(dButton);
			panel.Add(eButton);
		}

		private sealed class _ActionListener_804 : ActionListener
		{
			public _ActionListener_804(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.SetDyno();
				if (this._enclosing.loadFileCB.IsSelected())
				{
					this._enclosing.recordDataButton.SetText("Load From File");
				}
				else
				{
					this._enclosing.recordDataButton.SetText("Record Data");
				}
				this._enclosing.unitsPanel.SetVisible(true);
				this._enclosing.iPanel.SetVisible(true);
				this._enclosing.parent.Repaint();
			}

			private readonly DynoControlPanel _enclosing;
		}

		private sealed class _ActionListener_822 : ActionListener
		{
			public _ActionListener_822(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.chartPanel.SetET();
				this._enclosing.recordDataButton.SetText("Record ET");
				this._enclosing.unitsPanel.SetVisible(false);
				this._enclosing.iPanel.SetVisible(false);
				this._enclosing.parent.Repaint();
			}

			private readonly DynoControlPanel _enclosing;
		}

		private void BuildRadioButtons(JPanel panel)
		{
			iButton.AddActionListener(new _ActionListener_843(this));
			//            iButton.setActionCommand(IMPERIAL);
			iButton.SetSelected(true);
			mButton.AddActionListener(new _ActionListener_851(this));
			//            mButton.setActionCommand(METRIC);
			//            final JRadioButton sButton = new JRadioButton(SI);
			//            sButton.addActionListener(new ActionListener() {
			//                public void actionPerformed(ActionEvent actionEvent) {
			//                    buttonAction(sButton);
			//                }
			//            });
			//            sButton.setActionCommand(SI);
			//Group the radio buttons.
			ButtonGroup group = new ButtonGroup();
			group.Add(iButton);
			group.Add(mButton);
			//            group.add(sButton);
			panel.Add(iButton);
			panel.Add(mButton);
		}

		private sealed class _ActionListener_843 : ActionListener
		{
			public _ActionListener_843(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.ButtonAction(this._enclosing.iButton);
			}

			private readonly DynoControlPanel _enclosing;
		}

		private sealed class _ActionListener_851 : ActionListener
		{
			public _ActionListener_851(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.ButtonAction(this._enclosing.mButton);
			}

			private readonly DynoControlPanel _enclosing;
		}

		//            panel.add(sButton);
		private void ButtonAction(JRadioButton button)
		{
			double result = 0;
			units = button.GetActionCommand();
			if (units.Equals(IMPERIAL))
			{
				if (preUnits.Equals(METRIC))
				{
					result = ParseDouble(ambTemp) * 9 / 5 + 32;
					ambTemp.SetText(string.Format("%1.0f", result));
					result = ParseDouble(carMass) / 0.4536;
					carMass.SetText(string.Format("%1.0f", result));
					result = ParseDouble(deltaMass) / 0.4536;
					deltaMass.SetText(string.Format("%1.0f", result));
					result = ParseDouble(elevation) / 0.3048;
					elevation.SetText(string.Format("%1.0f", result));
					atm = atm / 6.89475728;
				}
				//            if (preUnits.equals(SI)){
				//                result = parseDouble(ambTemp)* 9/5 - 459.67;
				//                ambTemp.setText(String.format("%1.0f", result));
				//                result = parseDouble(carMass) / 0.4536;
				//                carMass.setText(String.format("%1.0f", result));
				//                result = parseDouble(deltaMass) / 0.4536;
				//                deltaMass.setText(String.format("%1.0f", result));
				//                result = parseDouble(elevation) / 0.3048;
				//                elevation.setText(String.format("%1.0f", result));
				//            }
				preUnits = IMPERIAL;
				elevUnits = "ft";
				tempUnits = "\u00b0F";
				elevLabel.SetText("Elevation (ft)");
				tempLabel.SetText("Air Temperature (\u00b0F)");
				deltaMassLabel.SetText("Delta Weight (lbs)");
				carMassLabel.SetText("Base Weight (lbs)");
				pressText = string.Format("%1.2f", atm);
				pressUnits = "psi";
			}
			if (units.Equals(METRIC))
			{
				if (preUnits.Equals(IMPERIAL))
				{
					result = (ParseDouble(ambTemp) - 32) * 5 / 9;
					ambTemp.SetText(string.Format("%1.1f", result));
					result = ParseDouble(carMass) * 0.4536;
					carMass.SetText(string.Format("%1.0f", result));
					result = ParseDouble(deltaMass) * 0.4536;
					deltaMass.SetText(string.Format("%1.0f", result));
					result = ParseDouble(elevation) * 0.3048;
					elevation.SetText(string.Format("%1.0f", result));
					atm = atm * 6.89475728;
				}
				//            if (preUnits.equals(SI)){
				//                result = parseDouble(ambTemp) - 273.15;
				//                ambTemp.setText(String.format("%1.1f", result));
				//            }
				preUnits = METRIC;
				elevUnits = "m";
				tempUnits = "\u00b0C";
				elevLabel.SetText("Elevation (m)");
				tempLabel.SetText("Air Temperature (\u00b0C)");
				deltaMassLabel.SetText("Delta Weight (kg)");
				carMassLabel.SetText("Base Weight (kg)");
				pressText = string.Format("%1.2f", atm);
				pressUnits = "kPa";
			}
			//        if (units.equals(SI)) {
			//            if (preUnits.equals(IMPERIAL)){
			//                result = (parseDouble(ambTemp) + 459.67) * 5/9;
			//                ambTemp.setText(String.format("%1.1f", result));
			//                result = parseDouble(carMass) * 0.4536;
			//                carMass.setText(String.format("%1.0f", result));
			//                LOGGER.trace("units selcted: " + units + " result: " + result);
			//                result = parseDouble(deltaMass) * 0.4536;
			//                deltaMass.setText(String.format("%1.0f", result));
			//                result = parseDouble(elevation) * 0.3048;
			//                elevation.setText(String.format("%1.0f", result));
			//            }
			//            if (preUnits.equals(METRIC)){
			//                result = parseDouble(ambTemp) + 273.15;
			//                ambTemp.setText(String.format("%1.1f", result));
			//            }
			//            preUnits = SI;
			//            elevUnits = "m";
			//            tempUnits = "K";
			//            elevLabel.setText("Elevation (m)");
			//            tempLabel.setText("Air Temperature (K)");
			//            deltaMassLabel.setText("Delta Weight (kg)");
			//            carMassLabel.setText("Base Weight (kg)");
			//        }
			if (resultStrings[0] != null)
			{
				interpolateButton.DoClick();
			}
			LOGGER.Info("DYNO Measurement units selected: " + units);
		}

		private void LoadFromFile()
		{
			JFileChooser openFile = new JFileChooser();
			if (path != null)
			{
				openFile.SetCurrentDirectory(new FilePath(path));
			}
			JButton openButton = new JButton("Open");
			int returnVal = openFile.ShowOpenDialog(openButton);
			if (returnVal == JFileChooser.APPROVE_OPTION)
			{
				FilePath logFile = openFile.GetSelectedFile();
				path = logFile.GetParent();
				BufferedReader inputStream = null;
				recordDataButton.SetSelected(false);
				chartPanel.ClearGraph();
				parent.Repaint();
				CalculateEnv();
				try
				{
					inputStream = new BufferedReader(new FileReader(logFile));
					LOGGER.Info("DYNO Opening log file: " + logFile.GetName());
					bool atrTime = false;
					double timeMult = 1;
					double startTime = -999999999;
					int timeCol = 0;
					int rpmCol = 0;
					int vsCol = 0;
					int taCol = 0;
					double minRpm = 3500;
					double maxRpm = 0;
					string delimiter = COMMA;
					string line = inputStream.ReadLine();
					string[] headers;
					headers = line.Split(COMMA);
					if (headers.Length < 3)
					{
						headers = line.Split(TAB);
						if (headers.Length > 2)
						{
							delimiter = TAB;
						}
						else
						{
							headers = line.Split(SEMICOLON);
							if (headers.Length > 2)
							{
								delimiter = SEMICOLON;
							}
						}
					}
					for (int x = 0; x < headers.Length; x++)
					{
						if (headers[x].Contains(RR_LOG_TIME))
						{
							timeCol = x;
						}
						if (headers[x].Contains(COBB_AP_TIME) || headers[x].Contains(AEM_LOG_TIME) || headers
							[x].Contains(OP2_LOG_TIME))
						{
							timeCol = x;
							timeMult = 1000;
						}
						if (headers[x].Contains(COBB_ATR_TIME))
						{
							timeCol = x;
							timeMult = 1000;
							atrTime = true;
						}
						if (headers[x].ToUpper().Contains(LOG_RPM) || headers[x].Contains(LOG_ES))
						{
							rpmCol = x;
						}
						if (headers[x].Contains(LOG_TA))
						{
							taCol = x;
						}
						if (headers[x].Contains(LOG_VS))
						{
							vsCol = x;
						}
						if (headers[x].Contains(LOG_VS_I))
						{
							vsLogUnits = LOG_VS_I;
						}
						if (headers[x].Contains(LOG_VS_M))
						{
							vsLogUnits = LOG_VS_M;
						}
					}
					LOGGER.Trace("DYNO log file conversions: Time Column: " + timeCol + "; Time X: " 
						+ timeMult + "; RPM Column: " + rpmCol + "; TA Column: " + taCol + "; VS Column: "
						 + vsCol + "; VS units: " + vsLogUnits);
					while ((line = inputStream.ReadLine()) != null)
					{
						string[] values = line.Split(delimiter);
						if (double.ParseDouble(values[taCol]) > 98)
						{
							double logTime = 0;
							if (atrTime)
							{
								string[] timeStamp = values[timeCol].Split(COLON);
								if (timeStamp.Length == 3)
								{
									logTime = ((double.ParseDouble(timeStamp[0]) * 3600) + (double.ParseDouble(timeStamp
										[1]) * 60) + double.ParseDouble(timeStamp[2])) * timeMult;
								}
								else
								{
									logTime = ((double.ParseDouble(timeStamp[0]) * 60) + double.ParseDouble(timeStamp
										[1])) * timeMult;
								}
							}
							else
							{
								logTime = double.ParseDouble(values[timeCol]) * timeMult;
							}
							if (startTime == -999999999)
							{
								startTime = logTime;
							}
							logTime = logTime - startTime;
							double logRpm = 0;
							if (IsManual())
							{
								logRpm = double.ParseDouble(values[rpmCol]);
								minRpm = Math.Min(minRpm, logRpm);
								maxRpm = Math.Max(maxRpm, logRpm);
							}
							else
							{
								logRpm = double.ParseDouble(values[vsCol]);
								minRpm = Math.Min(minRpm, SpeedCalculator.CalculateRpm(logRpm, rpm2mph, vsLogUnits
									));
								maxRpm = Math.Max(maxRpm, SpeedCalculator.CalculateRpm(logRpm, rpm2mph, vsLogUnits
									));
							}
							chartPanel.AddRawData(logTime, logRpm);
							LOGGER.Trace("DYNO log file time: " + logTime + "; speed: " + logRpm);
						}
					}
					inputStream.Close();
					rpmMin.SetText(string.Format("%1.0f", minRpm));
					rpmMax.SetText(string.Format("%1.0f", maxRpm));
					interpolateButton.DoClick();
				}
				catch (IOException)
				{
					if (inputStream != null)
					{
						try
						{
							inputStream.Close();
						}
						catch (IOException e1)
						{
							Sharpen.Runtime.PrintStackTrace(e1);
						}
					}
				}
			}
			else
			{
				LOGGER.Info("DYNO Open log file command cancelled by user.");
			}
		}

		private JButton BuildOpenReferenceButton()
		{
			JFileChooser openFile = new JFileChooser();
			if (path != null)
			{
				openFile.SetCurrentDirectory(new FilePath(path));
			}
			JButton openButton = new JButton("Open");
			openButton.AddActionListener(new _ActionListener_1088(this, openFile, openButton)
				);
			return openButton;
		}

		private sealed class _ActionListener_1088 : ActionListener
		{
			public _ActionListener_1088(DynoControlPanel _enclosing, JFileChooser openFile, JButton
				 openButton)
			{
				this._enclosing = _enclosing;
				this.openFile = openFile;
				this.openButton = openButton;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				int returnVal = openFile.ShowOpenDialog(openButton);
				if (returnVal == JFileChooser.APPROVE_OPTION)
				{
					FilePath traceFile = openFile.GetSelectedFile();
					this._enclosing.path = traceFile.GetParent();
					BufferedReader inputStream = null;
					this._enclosing.chartPanel.ClearRefTrace();
					try
					{
						inputStream = new BufferedReader(new FileReader(traceFile));
						RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Opening trace file: "
							 + traceFile.GetName());
						string line = inputStream.ReadLine();
						string[] refStats = line.Split(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.
							TAB);
						this._enclosing.reFfToE = double.ParseDouble(refStats[3]);
						this._enclosing.reFsToE = double.ParseDouble(refStats[4]);
						this._enclosing.reFtToS = double.ParseDouble(refStats[5]);
						this._enclosing.reFauc = double.ParseDouble(refStats[6]);
						RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Reference Stats: "
							 + Arrays.ToString(refStats));
						while ((line = inputStream.ReadLine()) != null)
						{
							string[] values = line.Split(RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.COMMA
								);
							if (double.ParseDouble(values[0]) >= 0)
							{
								this._enclosing.chartPanel.SetRefTrace(double.ParseDouble(values[0]), double.ParseDouble
									(values[1]), double.ParseDouble(values[2]));
							}
						}
						inputStream.Close();
						if (Sharpen.Runtime.EqualsIgnoreCase(refStats[0], RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel
							.IMPERIAL))
						{
							this._enclosing.iButton.SetSelected(true);
							this._enclosing.ButtonAction(this._enclosing.iButton);
						}
						if (Sharpen.Runtime.EqualsIgnoreCase(refStats[0], RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel
							.METRIC))
						{
							this._enclosing.mButton.SetSelected(true);
							this._enclosing.ButtonAction(this._enclosing.mButton);
						}
						this._enclosing.chartPanel.UpdateRefTrace(refStats);
					}
					catch (IOException)
					{
						if (inputStream != null)
						{
							try
							{
								inputStream.Close();
							}
							catch (IOException e1)
							{
								Sharpen.Runtime.PrintStackTrace(e1);
							}
						}
					}
				}
				else
				{
					RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Open trace file command cancelled by user."
						);
				}
			}

			private readonly DynoControlPanel _enclosing;

			private readonly JFileChooser openFile;

			private readonly JButton openButton;
		}

		private JButton BuildSaveReferenceButton()
		{
			JFileChooser openFile = new JFileChooser();
			if (path != null)
			{
				openFile.SetCurrentDirectory(new FilePath(path));
			}
			JButton saveButton = new JButton("Save");
			saveButton.AddActionListener(new _ActionListener_1148(this, openFile, saveButton)
				);
			return saveButton;
		}

		private sealed class _ActionListener_1148 : ActionListener
		{
			public _ActionListener_1148(DynoControlPanel _enclosing, JFileChooser openFile, JButton
				 saveButton)
			{
				this._enclosing = _enclosing;
				this.openFile = openFile;
				this.saveButton = saveButton;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				int returnVal = openFile.ShowSaveDialog(saveButton);
				if (returnVal == JFileChooser.APPROVE_OPTION)
				{
					FilePath traceFile = openFile.GetSelectedFile();
					this._enclosing.path = traceFile.GetParent();
					BufferedWriter outputStream = null;
					try
					{
						if (this._enclosing.dButton.IsSelected())
						{
							outputStream = new BufferedWriter(new FileWriter(traceFile));
							RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Saving trace to file: "
								 + traceFile.GetName());
							string line = this._enclosing.units + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel
								.TAB + this._enclosing.orderComboBox.GetSelectedItem() + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel
								.TAB + this._enclosing.resultStrings[1] + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel
								.TAB + this._enclosing.fToE + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.
								TAB + this._enclosing.sToE + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB
								 + this._enclosing.tToS + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB 
								+ this._enclosing.auc;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
							for (int x = 0; x < this._enclosing.chartPanel.GetPwrTqCount(); x++)
							{
								line = this._enclosing.chartPanel.GetPwrTq(x);
								outputStream.Write(line, 0, line.Length);
								outputStream.NewLine();
							}
						}
						if (this._enclosing.eButton.IsSelected())
						{
							outputStream = new BufferedWriter(new FileWriter(traceFile));
							RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Saving ET to file: "
								 + traceFile.GetName());
							string line = this._enclosing.carInfo;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
							line = "60ft/18.3m:" + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB + string
								.Format("%1.3f", this._enclosing.etResults[0]) + "\" @ " + string.Format("%1.2f"
								, this._enclosing.etResults[1]) + " " + this._enclosing.vsLogUnits;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
							line = "330ft/100m:" + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB + string
								.Format("%1.3f", this._enclosing.etResults[2]) + "\" @ " + string.Format("%1.2f"
								, this._enclosing.etResults[3]) + " " + this._enclosing.vsLogUnits;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
							line = "1/2 track:" + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB + string
								.Format("%1.3f", this._enclosing.etResults[4]) + "\" @ " + string.Format("%1.2f"
								, this._enclosing.etResults[5]) + " " + this._enclosing.vsLogUnits;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
							line = "1,000ft/305m:" + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB + 
								string.Format("%1.3f", this._enclosing.etResults[6]) + "\" @ " + string.Format("%1.2f"
								, this._enclosing.etResults[7]) + " " + this._enclosing.vsLogUnits;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
							line = "1/4 mile/402m:" + RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.TAB +
								 string.Format("%1.3f", this._enclosing.etResults[8]) + "\" @ " + string.Format(
								"%1.2f", this._enclosing.etResults[9]) + " " + this._enclosing.vsLogUnits;
							outputStream.Write(line, 0, line.Length);
							outputStream.NewLine();
						}
						outputStream.Close();
					}
					catch (IOException)
					{
						try
						{
							outputStream.Close();
						}
						catch (IOException e1)
						{
							Sharpen.Runtime.PrintStackTrace(e1);
						}
					}
				}
				else
				{
					RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Save trace file command cancelled by user."
						);
				}
			}

			private readonly DynoControlPanel _enclosing;

			private readonly JFileChooser openFile;

			private readonly JButton saveButton;
		}

		private JButton BuildClearReferenceButton()
		{
			JButton clearButton = new JButton("Clear");
			clearButton.AddActionListener(new _ActionListener_1217(this));
			return clearButton;
		}

		private sealed class _ActionListener_1217 : ActionListener
		{
			public _ActionListener_1217(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.reFfToE = 0;
				this._enclosing.reFsToE = 0;
				this._enclosing.reFtToS = 0;
				this._enclosing.reFauc = 0;
				this._enclosing.chartPanel.ClearRefTrace();
				if (this._enclosing.results[0] > 0)
				{
					this._enclosing.interpolateButton.DoClick();
				}
			}

			private readonly DynoControlPanel _enclosing;
		}

		private void RegisterData(params string[] ids)
		{
			foreach (string id in ids)
			{
				LoggerData data = FindData(id);
				EcuDataConvertor convertor = data.GetSelectedConvertor();
				string convertorUnits = convertor.GetUnits();
				if (id.Equals(ATM))
				{
					atmLogUnits = convertorUnits;
				}
				if (id.Equals(IAT))
				{
					iatLogUnits = convertorUnits;
				}
				if (id.Equals(VEHICLE_SPEED))
				{
					vsLogUnits = convertorUnits;
				}
				if (data != null)
				{
					broker.RegisterLoggerDataForLogging(data);
				}
			}
		}

		private void DeregisterData(params string[] ids)
		{
			foreach (string id in ids)
			{
				LoggerData data = FindData(id);
				if (data != null)
				{
					broker.DeregisterLoggerDataFromLogging(data);
				}
			}
		}

		private LoggerData FindData(string id)
		{
			foreach (EcuParameter param in @params)
			{
				if (id.Equals(param.GetId()))
				{
					return param;
				}
			}
			foreach (EcuSwitch sw in switches)
			{
				if (id.Equals(sw.GetId()))
				{
					return sw;
				}
			}
			foreach (ExternalData external in externals)
			{
				if (id.Equals(external.GetId()))
				{
					return external;
				}
			}
			return null;
		}

		private void AddComponent(JPanel panel, GridBagLayout gridBagLayout, JComponent component
			, int y)
		{
			Add(panel, gridBagLayout, component, 0, y, 3, GridBagConstraints.NONE);
		}

		private void AddMinMaxFilter(JPanel panel, GridBagLayout gridBagLayout, string name
			, JTextField min, JTextField max, int y)
		{
			Add(panel, gridBagLayout, new JLabel(name), 0, y, 3, GridBagConstraints.HORIZONTAL
				);
			y += 1;
			Add(panel, gridBagLayout, min, 0, y, 2, GridBagConstraints.NONE);
			Add(panel, gridBagLayout, new JLabel(" - "), 1, y, 0, GridBagConstraints.NONE);
			Add(panel, gridBagLayout, max, 2, y, 1, GridBagConstraints.NONE);
		}

		private GridBagConstraints BuildBaseConstraints()
		{
			GridBagConstraints constraints = new GridBagConstraints();
			constraints.anchor = GridBagConstraints.CENTER;
			constraints.fill = GridBagConstraints.NONE;
			return constraints;
		}

		private void UpdateConstraints(GridBagConstraints constraints, int gridx, int gridy
			, int gridwidth, int gridheight, int weightx, int weighty, int fill)
		{
			constraints.gridx = gridx;
			constraints.gridy = gridy;
			constraints.gridwidth = gridwidth;
			constraints.gridheight = gridheight;
			constraints.weightx = weightx;
			constraints.weighty = weighty;
			constraints.fill = fill;
		}

		private JButton BuildInterpolateButton(JComboBox orderComboBox)
		{
			interpolateButton.AddActionListener(new _ActionListener_1292(this));
			return interpolateButton;
		}

		private sealed class _ActionListener_1292 : ActionListener
		{
			public _ActionListener_1292(DynoControlPanel _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				if (this._enclosing.dButton.IsSelected())
				{
					this._enclosing.interpolateButton.SetEnabled(true);
					if (this._enclosing.IsValidRange(this._enclosing.rpmMin, this._enclosing.rpmMax))
					{
						this._enclosing.CalculateEnv();
						this._enclosing.UpdateChart();
					}
					else
					{
						JOptionPane.ShowMessageDialog(this._enclosing.parent, "Invalid PRM range specified."
							, "Error", JOptionPane.ERROR_MESSAGE);
					}
				}
			}

			private readonly DynoControlPanel _enclosing;
		}

		private JComboBox BuildPolyOrderComboBox()
		{
			JComboBox orderComboBox = new JComboBox(new object[] { 5, 6, 7, 8, 9, 10, 11, 12, 
				13, 14, 15, 16, 17, 18, 19 });
			orderComboBox.SetSelectedItem(9);
			return orderComboBox;
		}

		private bool AreNumbers(params JTextField[] textFields)
		{
			foreach (JTextField field in textFields)
			{
				if (!IsNumber(field))
				{
					return false;
				}
			}
			return true;
		}

		private bool CheckInRange(string name, JTextField min, JTextField max, double value
			)
		{
			if (IsValidRange(min, max))
			{
				return InRange(value, min, max);
			}
			else
			{
				return false;
			}
		}

		private bool IsValidRange(JTextField min, JTextField max)
		{
			return AreNumbers(min, max) && ParseDouble(min) < ParseDouble(max);
		}

		private bool InRange(double value, JTextField min, JTextField max)
		{
			return InRange(value, ParseDouble(min), ParseDouble(max));
		}

		private bool InRange(double val, double min, double max)
		{
			return val >= min && val <= max;
		}

		private bool IsNumber(JTextField textField)
		{
			try
			{
				ParseDouble(textField);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private double ParseDouble(JTextField field)
		{
			return double.ParseDouble(field.GetText().Trim());
		}

		public void SetEcuParams(IList<EcuParameter> @params)
		{
			this.@params = new AList<EcuParameter>(@params);
		}

		public void SetEcuSwitches(IList<EcuSwitch> switches)
		{
			this.switches = new AList<EcuSwitch>(switches);
		}

		public void SetExternalDatas(IList<ExternalData> externals)
		{
			this.externals = new AList<ExternalData>(externals);
		}

		private JComboBox BuildCarSelectComboBox()
		{
			LoadCars();
			JComboBox selectComboBox = new JComboBox(carTypeArr);
			selectComboBox.AddActionListener(new _ActionListener_1369(this, selectComboBox));
			//        carSelectBox.setSelectedItem("05 USDM OBXT WGN LTD 5MT");
			return selectComboBox;
		}

		private sealed class _ActionListener_1369 : ActionListener
		{
			public _ActionListener_1369(DynoControlPanel _enclosing, JComboBox selectComboBox
				)
			{
				this._enclosing = _enclosing;
				this.selectComboBox = selectComboBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.ChangeCars(selectComboBox.GetSelectedIndex());
			}

			private readonly DynoControlPanel _enclosing;

			private readonly JComboBox selectComboBox;
		}

		private JComboBox BuildGearComboBox()
		{
			//        makeGearList();
			JComboBox gearSelectBox = new JComboBox();
			gearSelectBox.AddActionListener(new _ActionListener_1381(this, gearSelectBox));
			return gearSelectBox;
		}

		private sealed class _ActionListener_1381 : ActionListener
		{
			public _ActionListener_1381(DynoControlPanel _enclosing, JComboBox gearSelectBox)
			{
				this._enclosing = _enclosing;
				this.gearSelectBox = gearSelectBox;
			}

			public void ActionPerformed(ActionEvent actionEvent)
			{
				this._enclosing.gearRatio.SetText(this._enclosing.gearsRatioArr[this._enclosing.carSelectBox
					.GetSelectedIndex()][gearSelectBox.GetSelectedIndex() + 1]);
				RomRaider.Logger.Ecu.UI.Tab.Dyno.DynoControlPanel.LOGGER.Info("DYNO Car: " + this
					._enclosing.carSelectBox.GetSelectedItem() + ", Changed gear to: " + gearSelectBox
					.GetSelectedItem() + " (" + this._enclosing.gearRatio.GetText() + ")");
			}

			private readonly DynoControlPanel _enclosing;

			private readonly JComboBox gearSelectBox;
		}

		private void MakeGearList()
		{
			gearList = new string[Sharpen.Extensions.ValueOf(gearsRatioArr[carSelectBox.GetSelectedIndex
				()][0])];
			for (int g = 1; g <= Sharpen.Extensions.ValueOf(gearsRatioArr[carSelectBox.GetSelectedIndex
				()][0]); g++)
			{
				gearList[g - 1] = Sharpen.Extensions.ToString(g);
			}
		}

		private void ChangeCars(int index)
		{
			if (!carTypeArr[0].Trim().Equals(MISSING_CAR_DEF))
			{
				iButton.DoClick();
				carMass.SetText(carMassArr[index]);
				dragCoeff.SetText(dragCoeffArr[index]);
				rollCoeff.SetText(rollCoeffArr[index]);
				frontalArea.SetText(frontalAreaArr[index]);
				if (gearsRatioArr[index][0] == null)
				{
					gearRatio.SetText(gearRatioArr[index]);
				}
				else
				{
					//            gearRatio.setText(gearsRatioArr[index][gearSelectBox.getSelectedIndex() + 1]);
					gearRatio.SetText(gearsRatioArr[index][1]);
				}
				finalRatio.SetText(finalRatioArr[index]);
				transmission.SetText(transArr[index]);
				tireWidth.SetText(widthArr[index]);
				tireAspect.SetText(aspectArr[index]);
				tireSize.SetText(sizeArr[index]);
				MakeGearList();
				LOGGER.Info("DYNO New Car Selected: " + carSelectBox.GetSelectedItem() + ", Gears: "
					 + gearsRatioArr[carSelectBox.GetSelectedIndex()][0]);
				if (gearList == null)
				{
					gearSelectBox.SetModel(new DefaultComboBoxModel());
				}
				else
				{
					int gear = System.Convert.ToInt32((gearsRatioArr[carSelectBox.GetSelectedIndex()]
						[0])) - 3;
					gearSelectBox.SetModel(new DefaultComboBoxModel(gearList));
					gearSelectBox.SetSelectedIndex(gear);
				}
			}
		}

		private void LoadCars()
		{
			try
			{
				FilePath carDef = null;
				string SEPARATOR = Runtime.GetProperty("file.separator");
				string loggerFilePath = ECUExec.settings.GetLoggerDefFilePath();
				if (loggerFilePath != null)
				{
					int index = loggerFilePath.LastIndexOf(SEPARATOR);
					if (index > 0)
					{
						string path = Sharpen.Runtime.Substring(loggerFilePath, 0, index + 1);
						carDef = new FilePath(path + CARS_FILE);
					}
				}
				if (!carDef.Exists())
				{
					string profileFilePath = Settings.GetLoggerProfileFilePath();
					if (profileFilePath != null)
					{
						int index = profileFilePath.LastIndexOf(SEPARATOR);
						if (index > 0)
						{
							string path = Sharpen.Runtime.Substring(profileFilePath, 0, index + 1);
							carDef = new FilePath(path + CARS_FILE);
						}
					}
				}
				if (!carDef.Exists())
				{
					carDef = new FilePath(CARS_FILE);
				}
				if (!carDef.Exists())
				{
					throw new FileNotFoundException(MISSING_CAR_DEF);
				}
				DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.NewInstance();
				DocumentBuilder docBuilder = docBuilderFactory.NewDocumentBuilder();
				Document carsDef = docBuilder.Parse(carDef);
				// normalize text representation
				carsDef.GetDocumentElement().Normalize();
				NodeList listOfCars = carsDef.GetElementsByTagName("car");
				int totalCars = listOfCars.GetLength();
				carTypeArr = new string[totalCars];
				carMassArr = new string[totalCars];
				dragCoeffArr = new string[totalCars];
				rollCoeffArr = new string[totalCars];
				frontalAreaArr = new string[totalCars];
				gearRatioArr = new string[totalCars];
				//gearsRatioArr = new String[totalCars][7];
				finalRatioArr = new string[totalCars];
				transArr = new string[totalCars];
				widthArr = new string[totalCars];
				aspectArr = new string[totalCars];
				sizeArr = new string[totalCars];
				string[] tag = new string[] { "type", "carmass", "dragcoeff", "rollcoeff", "frontalarea"
					, "finalratio", "transmission", "tirewidth", "tireaspect", "wheelsize" };
				for (int s = 0; s < listOfCars.GetLength(); s++)
				{
					Node carNode = listOfCars.Item(s);
					if (carNode.GetNodeType() == Node.ELEMENT_NODE)
					{
						Element carElement = (Element)carNode;
						// Read element types and populate arrays
						for (int i = 0; i < tag.Length; i++)
						{
							NodeList list = carElement.GetElementsByTagName(tag[i]);
							Element element = (Element)list.Item(0);
							if (element != null)
							{
								NodeList value = element.GetChildNodes();
								string data = ((Node)value.Item(0)).GetNodeValue().Trim();
								switch (i)
								{
									case 0:
									{
										carTypeArr[s] = data;
										//                                    gearRatioArr[s] = data;
										for (int g = 1; g <= 6; g++)
										{
											string gearNo = "gearratio" + g;
											NodeList grsList = carElement.GetElementsByTagName(gearNo);
											Element carGrsElement = (Element)grsList.Item(0);
											if (carGrsElement != null)
											{
												NodeList grsValueList = carGrsElement.GetChildNodes();
												if (((Node)grsValueList.Item(0)).GetNodeValue().Trim() != null)
												{
													gearsRatioArr[s][0] = Sharpen.Extensions.ToString(g);
													gearsRatioArr[s][g] = (string)((Node)grsValueList.Item(0)).GetNodeValue().Trim();
												}
											}
										}
										//                                        LOGGER.trace("Car: " + s + " Gear: " + g + " Ratio: " + gearsRatioArr[s][g]);
										break;
									}

									case 1:
									{
										carMassArr[s] = data;
										break;
									}

									case 2:
									{
										dragCoeffArr[s] = data;
										break;
									}

									case 3:
									{
										rollCoeffArr[s] = data;
										break;
									}

									case 4:
									{
										frontalAreaArr[s] = data;
										break;
									}

									case 5:
									{
										finalRatioArr[s] = data;
										break;
									}

									case 6:
									{
										transArr[s] = data;
										break;
									}

									case 7:
									{
										widthArr[s] = data;
										break;
									}

									case 8:
									{
										aspectArr[s] = data;
										break;
									}

									case 9:
									{
										sizeArr[s] = data;
										break;
									}
								}
							}
						}
					}
				}
			}
			catch (SAXParseException err)
			{
				JOptionPane.ShowMessageDialog(parent, "Parsing error" + ", line " + err.GetLineNumber
					() + ", " + err.GetSystemId() + ".\n" + err.Message, "Error", JOptionPane.ERROR_MESSAGE
					);
				LOGGER.Error("DYNO ** Parsing error" + ", line " + err.GetLineNumber() + ", uri "
					 + err.GetSystemId());
				LOGGER.Error(" " + err.Message);
			}
			catch (SAXException e)
			{
				Exception x = e.GetException();
				Sharpen.Runtime.PrintStackTrace(((x == null) ? e : x));
			}
			catch (Exception t)
			{
				// file not found
				object[] options = new object[] { "Yes", "No" };
				int answer = JOptionPane.ShowOptionDialog(this, "Cars definition file not found.\nGo online to download the latest definition file?"
					, "Configuration", JOptionPane.DEFAULT_OPTION, JOptionPane.WARNING_MESSAGE, null
					, options, options[0]);
				if (answer == 0)
				{
					BareBonesBrowserLaunch.OpenURL(Version.CARS_DEFS_URL);
				}
				else
				{
					JOptionPane.ShowMessageDialog(parent, MISSING_CAR_DEF + " file from the installation or profiles or definitions directory.\nDyno feature will not be available until this file is present."
						, "Notice", JOptionPane.WARNING_MESSAGE);
				}
				carTypeArr = new string[] { MISSING_CAR_DEF };
				Sharpen.Runtime.PrintStackTrace(t);
			}
		}

		private static void SetSelectAllFieldText(JTextComponent comp)
		{
			// Ensures that all the text in a JTextComponent will be
			// selected whenever the cursor is in that field (gains focus):
			if (allTextSelector == null)
			{
				allTextSelector = new _FocusAdapter_1572();
			}
			comp.AddFocusListener(allTextSelector);
		}

		private sealed class _FocusAdapter_1572 : FocusAdapter
		{
			public _FocusAdapter_1572()
			{
			}

			public override void FocusGained(FocusEvent ev)
			{
				JTextComponent textComp = (JTextComponent)ev.GetSource();
				textComp.SelectAll();
			}
		}
	}
}
