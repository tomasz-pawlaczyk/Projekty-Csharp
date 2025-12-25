# NYC Bike Rentals 2015 — Data Analysis in C#

## 📌 Project Overview

This project analyzes **NYC Citi Bike rental data from 2015** using a C# console application.  
The goal is to extract **usage patterns**, **behavioral differences between users**, **seasonal trends**, and **operational insights** that could help improve a city bike-sharing system.

The analysis processes **nearly 10 million trips** and combines trip data with **weather conditions** to better understand demand and anomalies.

---

## 📂 Data Sources

- **Citi Bike trip data (2015)**  
  CSV fields:  
  `tripduration, starttime, stoptime, start station id, start station name, start station latitude, start station longitude, end station id, end station name, end station latitude, end station longitude, bikeid, usertype, birth year, gender`

- **NYC Weather data (2015)**  
  CSV fields:  
    `date, temperature, weather_event`

> CSV files are intentionally **not included in the repository** due to their large size.

---

## 📊 Selected Results & Interpretation

### I. Gender Usage Patterns

- **Men:** ~77% of all rides
- **Women:** ~23% of all rides
- **Average trip duration:** women ride noticeably longer

**Interpretation:**  
This suggests different usage styles — women may use bikes more recreationally, while men more often for commuting.

---

### II. Time & Seasonality Analysis

- Clear **commuting peaks**:
  - Morning: 08–10
  - Afternoon: 16–18
- Strong **seasonal effect**:
  - Winter months show low usage
  - Summer and early autumn peak strongly

**Interpretation:**  
Bike sharing in NYC is heavily weather- and daylight-dependent and closely aligned with work schedules.

---

### III. Trips vs Weather

- Bike usage rises sharply with temperature
- Snow and mixed weather conditions drastically reduce demand

**Interpretation:**  
Weather is one of the strongest predictors of demand and can be used for dynamic pricing, maintenance scheduling, and fleet redistribution planning.

---

### IV. Rebalancing the Bike Fleet (Operational Insight)

**Stations losing bikes (example):**

```
8 Ave & W 31 St        | -47.97 bikes/day
W 42 St & 8 Ave        | -38.65 bikes/day
Pershing Square South  | -32.27 bikes/day
```

**Stations gaining bikes (example):**

```
W 33 St & 7 Ave         | +32.81 bikes/day
DeKalb Ave & Hudson Ave | +22.60 bikes/day
W 52 St & 5 Ave         | +16.76 bikes/day
```

**Interpretation:**  
These results directly indicate where bikes should be transported from and to in order to optimize availability and reduce shortages during peak hours.

---

### V. Compare Popular Routes

The analysis distinguishes between three types of routes:

- **Loops** (same start and end station)
- **Directed routes** (A → B)
- **Undirected routes** (A ↔ B, direction ignored)

**Top routes including loops** are dominated by locations around parks and tourist areas, for example:

- *Central Park S & 6 Ave → Central Park S & 6 Ave*
- *Broadway & W 60 St → Broadway & W 60 St*

This indicates strong recreational usage, where users rent a bike, ride around an area, and return it to the same station.

---

### VI. Customer vs Subscriber Behavior

- **Subscribers**:
  
  - shorter trips
  - peak during commuting hours
  - mostly point A → B travel

- **Customers**:
  
  - longer trips
  - more daytime usage
  - higher share of loops (same start/end station)

**Interpretation:**  
Customers are likely tourists or recreational users, while subscribers are city residents using bikes primarily for transportation.

---

### VII. Anomaly Detection

#### Long Trips with Zero Distance

Examples:

```
E 11 St & 2 Ave | 34064 min | 0.00 km
2 Ave & E 58 St | 10232 min | 0.00 km
```

**Possible causes:**

- bike malfunction
- docking issues
- rental not properly ended
- system or sensor errors

---

#### Unrealistic Speeds

Some trips imply speeds exceeding **50-60 km/h**.

**Interpretation:**  
These values are data anomalies and may indicate bicycle transportation by service vehicles, or result from timestamp inconsistencies, GPS station errors, or delayed trip closures.

---

#### Bike Usage Extremes

**Most used bikes:**

```
Bike 22123 : 1998 trips
Bike 21999 : 1981 trips
...
```

**Interpretation:**  
Highly used bikes should be prioritized for inspection and preventive maintenance to reduce failure rates.

---

## ⏱ Performance

- **Total execution time:** ~65 seconds
- **Dataset size:** ~10 million records
- Processing uses streaming file reads and efficient grouping operations

---

## 🧠 Key Takeaways

- Bike-sharing usage strongly depends on time, weather, and user type
- Real-world data contains inconsistencies that must be handled explicitly
- Simple analytics can provide direct operational value
- C# is fully capable of large-scale data analysis when designed carefully

---

*This project was created as part of academic coursework and focuses on practical data analysis and reasoning rather than UI or visualization*
