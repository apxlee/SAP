@echo off
set ver=%1

if %1.==. (
set ver=0.0.0
)else (
echo Build version: %ver%
)

nant -f:release\buildfiles\default.build -D:version=%ver% build.svn package
pause