# NetworkLimiter

A cross-platform network monitoring and bandwidth control application built with .NET 8.0, providing real-time network statistics, bandwidth limiting capabilities, and privacy features for both Windows and Linux users.

## Table of Contents

- [Features](#features)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Usage](#usage)
  - [Starting the Application](#starting-the-application)
  - [Selecting Network Interface](#selecting-network-interface)
  - [Live Network Statistics](#live-network-statistics)
  - [Options Menu](#options-menu)
- [Privacy Mode](#privacy-mode)
- [Password Protection](#password-protection)
- [Screenshots](#screenshots)

---

## Features

- **Cross-Platform Compatibility**: Runs on both Windows and Linux systems using .NET 8.0.
- **Real-Time Network Monitoring**:
  - Displays detailed network interface information.
  - Provides live updates of bytes sent and received in megabytes per second (MB/s).
- **Bandwidth Limiting**:
  - Allows users to set speed limits (in Mbps) on selected network interfaces.
  - Option to remove speed limits at any time.
- **Privacy Mode**:
  - Hides sensitive network information (e.g., IP addresses, byte counts) when enabled.
- **Password Protection**:
  - Secure the application with a password to prevent unauthorized access.
- **User-Friendly Interface**:
  - Organized and color-coded console output for better readability.
  - Options menu accessible at any time by pressing 'M'.
- **Administrative Privileges Check**:
  - Ensures the application is run with the necessary permissions to manage network settings.

---

## Prerequisites

- **.NET 8.0 Runtime**: Ensure that the .NET 8.0 runtime is installed on your system.
  - [Download .NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)

---

## Installation

1. **Download the Latest Release**:
   - Go to the [Releases](https://github.com/Swishhyy/NetworkLimiter/releases) section and download the latest release suitable for your operating system (Windows or Linux).

2. **Extract the Files**:
   - Unzip the downloaded file to your desired location.

3. **Run the Application**:

   - **Windows**:
     - Navigate to the extracted folder.
     - Double-click `NetworkLimiter.exe` to start the application.

   - **Linux**:
     - Open a terminal and navigate to the extracted folder.
     - Run `chmod +x NetworkLimiter` to make the application executable.
     - Execute `./NetworkLimiter` in the terminal to start the application.

---

## Usage

### Starting the Application

- Upon running the application, it will check for administrator (Windows) or root (Linux) privileges.
- If a password is set, you'll be prompted to enter it.

### Selecting Network Interface

- The application will display a list of available network interfaces.
- Enter the number corresponding to the network interface you wish to monitor and manage.

### Live Network Statistics

- After selecting the network interface, the application displays detailed information and starts live updates.
- Information includes:
  - Interface details (Name, Description, Type, Status, Speed).
  - Live updates of bytes sent and received per second in MB/s.
- **Note**: Press **'M'** at any time to open the options menu.

### Options Menu

Press **'M'** to access the options menu, which includes:

1. **Limit Speeds**:
   - Set a speed limit (in Mbps) for the selected network interface.
   - The application calculates and applies the limit in bytes per second.

2. **Unlimit Speeds**:
   - Remove any speed limits applied to the network interface.

3. **Toggle Privacy Mode**:
   - Enable or disable privacy mode.
   - When enabled, sensitive data like IP addresses and byte counts are hidden.

4. **Set/Change Password**:
   - Protect the application with a password.
   - If a password is already set, you will need to enter the current password to change it.

5. **Return to Live Network Information**:
   - Go back to the main screen to view live updates.

6. **Exit**:
   - Close the application.

---

## Privacy Mode

- **Purpose**: Hide sensitive network information from the display.
- **Toggle**: Accessible through the options menu.
- **Effect**:
  - IP addresses and byte counts are not displayed.
  - A message indicates that vital data is hidden.

---

## Password Protection

- **Set a Password**:
  - Use the options menu to set a new password.
- **Change Password**:
  - If a password is already set, you can change it by entering the current password first.
- **Security**:
  - The password is stored securely and required on application startup.

---

### Screenshots

Select what network you want to manage:
![image](https://github.com/user-attachments/assets/d890d51c-4fb1-467f-b1f6-7ebae4685c5c)

Network Information (Im Running it in privacy mode but the non privacy mode shows your ipv4/ipv6 addresses) :
![image](https://github.com/user-attachments/assets/2d8f37b7-69a5-4f3b-892a-93a32b8685ab)

The options menu:
![image](https://github.com/user-attachments/assets/bcad9336-c18b-4330-bd21-da6729d2de79)



