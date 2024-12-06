# CubeSat Change Documentation
This file will be used for documenting any changes made as part of the CubeSat project.
At a minimum, it should list the files added/modified from the base Renode project. There should be additional documentation of the reasoning for any such changes if they are not well documented in the source of the file in question.

## Added:

- [`CubeSatChanges.md`](CubeSatChanges.md)
    - *This file. Added for above reasons*
- **src/**
    - **Emulator/Peripherals/**
        - **Peripherals/**
            - **DMA/**
                - [STM32DMA_Fixed.cs](src/Emulator/Peripherals/Peripherals/DMA/STM32DMA_Fixed.cs)
                    - Found from a [github issue](https://github.com/renode/renode/issues/616)
                    - Added in the hopes of addressing issues with startup
            - **I2C/**
                - [STMf4_I2C_Fixed.cs](src/Emulator/Peripherals/Peripherals/I2C/STM32_I2C_Fixed.cs)
                    - Found from a [github issue](https://github.com/renode/renode/issues/616)
                    - Added in the hopes of addressing issues with startup
            - **Miscellaneous/**
                - [CF_Syslink.cs](src/Emulator/Peripherals/Peripherals/Miscellaneous/CF_Syslink.cs)
                    - *Adds the device for the CF Syslink*
                - [EEPROM_24AA64.cs](src/Emulator/Peripherals/Peripherals/Miscellaneous/EEPROM_24AA64.cs)
                    - *Adds EEPROM*
                    - I've made some additional changes here for debugging
                    - This file appears to be the source of the problems with initialization of the baseline CF2 firmware
            - **Sensors/**
                - [BMI088_Accelerometer.cs](src/Emulator/Peripherals/Peripherals/Sensors/BMI088_Accelerometer.cs)
                    - *Adds CF2 accelerometer*
                - [BMI088_Gyroscope.cs](src/Emulator/Peripherals/Peripherals/Sensors/BMI088_Gyroscope.cs)
                    - *Adds CF2 gyroscope*
                - [BMP388.cs](src/Emulator/Peripherals/Peripherals/Sensors/BMP388.cs)
                    - *Adds CF2 barometer*


## Modified:

- **src/**
    - **Emulator/Peripherals/**
        - **Peripherals/**
            - [Peripherals.scproj](src/Emulator/Peripherals/Peripherals.csproj)
                - *Adds compile directives for other added files*
                - This shouldn't be needed, strictly speaking, as we're building the [.Net version](src/Emulator/Peripherals/Peripherals_NET.csproj) of the project. I've included it in case someone wants to, at some later point, work with non .Net builds.
