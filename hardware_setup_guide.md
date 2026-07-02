# Custom VR Hardware Setup Guide

This guide is designed for building your custom VR hardware using the HadesVR ecosystem, omitting any 3D printable parts or optics. The focus here is strictly on the electronics (PCBs), firmware flashing, and setting up the tracking using PSMoveServiceEx.

---

## 1. Controller Boards & Components

The custom PCB designs are available in the `Hardware/Controllers/Controller boards` directory.

> [!NOTE]
> The original PCB files can also be found at [oshwlab.com/liquidcgs/vrcontrollers_public](https://oshwlab.com/liquidcgs/vrcontrollers_public).

When you have your custom controller boards manufactured, you will likely receive them as a panel. You will need to snap them off so you're left with two pairs of controller boards.

### General Controller Hardware Needed (per controller):
*   **Arduino Nano / UNO / Pro Mini** (Or custom Atmega328p on the PCB)
*   **MPU9250** (9dof IMU for rotation data)
*   **NRF24L01** (For transmitting data; requires 3.3v)
*   **5mm LED (Red, Blue, Green, etc.)** (Used for 6dof tracking; each device needs a unique color)
*   **Tact switches** (For buttons like Trigger, Grip, Menu, System)
*   **Lithium battery & TP4056** (For power and charging)
*   **5v step up converter / 3.3v LDO** (Depending on your microcontroller voltage)
*   **Analog stick module**
*   **White ping pong ball (40mm)** (Used to diffuse the LED light for tracking)

---

## 2. Bootloader Installation (For Custom PCBs)

If you are using the custom boards with a bare Atmega328p, you will need to flash a custom bootloader that allows the use of the internal 8Mhz oscillator as a clock signal. 

> [!TIP]
> We recommend using [MiniCore](https://github.com/MCUdude/MiniCore). Instructions for adding it to the Arduino IDE can be found on their GitHub.

### Bootloader Minimum Component Requirement
Before flashing the bootloader, solder these components to your boards:

| Component Type | Left Board | Right Board | Value |
| :--- | :--- | :--- | :--- |
| Microcontroller | U3 | U9 | Atmega328p |
| Resistor | R12 | R11 | 10kΩ |
| 3.3v Regulator | U10 | U4 | 3.3v LDO |
| Capacitor | C19 | C12 | 1uF |
| Capacitor | C20 | C13 | 1uF |

### Flashing the Bootloader via Arduino as ISP
The easiest way to flash the bootloader is using another Arduino as an ISP (In-System Programmer). 

1. Upload the `ArduinoISP` sketch (found in Arduino IDE Examples) to your programmer Arduino.
2. Connect the SPI pins (MISO, MOSI, SCK) and Digital Pin 10 (to Reset) from the programmer Arduino to your custom board's SPI header.
3. Under the `Tools` menu, select `Boards` -> `MiniCore` -> `ATmega328`.
4. Use the following settings:
   - **Clock:** Internal 8MHz
   - **BOD:** 2.7V
   - **EEPROM:** EEPROM Retained
   - **LTO:** LTO Disabled
   - **Variant:** 328P / 328PA
   - **Bootloader:** Yes (UART0)
5. Select **"Burn Bootloader"**. If successful, the IDE will display "Done burning bootloader."

After burning the bootloader, you can upload the HadesVR firmware (from `Software/Firmware/`) using an FTDI programmer connected to the debug port.

---

## 3. Tracking Setup (PSMoveServiceEx)

Tracking is achieved using glowing LED blobs (diffused by ping pong balls) tracked by multiple PlayStation Eye cameras.

> [!IMPORTANT]
> You will need at least **2 PlayStation Eye cameras** for basic triangulation, though **3 to 4 are highly recommended** for redundancy to prevent occlusion issues.

### Installing Camera Drivers
1. Download and install [Zadig](https://zadig.akeo.ie/).
2. Plug in all your PS Eye cameras.
3. Open Zadig, select one of the USB cameras ("Interface 0" only), choose `libusb-win32` as the driver, and click **Install Driver**.
4. Repeat for all connected cameras.

### Installing and Configuring PSMSEx
1. Download [PSMoveServiceEx](https://github.com/Timocop/PSMoveServiceEx).
2. Run `PSMoveServiceAdmin.exe` for a few seconds to let it generate configuration files, then close it.
3. Press `Win + R` and navigate to `%appdata%\PSMoveService`.
4. Open `ControllerManagerConfig.json` and change `"virtual_controller_count"` to `2`.
5. Open `HMDManagerConfig.json` and change `"virtual_hmd_count"` to `1`.
6. Run `PSMoveServiceAdmin.exe` again briefly, then close it.
7. Open the newly generated tracker config files (e.g., `TrackerManagerConfig.json` or individual tracker configs) and ensure `"bulb_radius"` is set to `2.0` (since ping pong balls are 4cm in diameter, unlike official PSMove controllers which are 4.5cm).
8. Launch `PSMoveServiceAdmin.exe`, then open `PSMoveConfigTool.exe` and click Connect.
9. In Controller/HMD settings, assign the correct tracking color for each device (e.g., Green for HMD, Red for Left Controller, Blue for Right Controller).

### Room Setup Tips
* Mount cameras about 30cm (1 foot) above your head, angled slightly down to cover the floor.
* Avoid bright sunlight and lighting that matches your tracking blobs (like RGB keyboards).
* Ensure the fields of view overlap as much as possible for optimal redundancy.
