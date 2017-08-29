@echo off
setlocal ENABLEDELAYEDEXPANSION
netcfg /l %1% /c s /i brcloak
PAUSE