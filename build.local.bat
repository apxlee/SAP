rem svn update
nant -f:release\buildfiles\default.build -D:configuration=debug -D:version=0.0.0 build test
pause