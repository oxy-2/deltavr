
---
intro
Renode doesn't emulate every pin and electrical signal. It emulates the flow of data ONLY. So this means that the pin I2c1 for example, is a combination of both SDA and SCL, or in our case pins 5 and 6.

---
![[Pasted image 20261020164929.png]]![[Pasted image 20261020165010.png]]

---
SPI 1 = MOSI / MISO / SCK / CS

Used for NRF24LO1 / LSM6DSVTR

mosi - 16
miso - 19
sck - 17
cs for NRF24LO1 - 15
cs for LSM6DSVTR - 18

---
I2c1 = SDA / SCL

Used for ADS1115IDGS

sda - 5
scl - 6

---
