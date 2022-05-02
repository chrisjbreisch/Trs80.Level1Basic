10 input " Starting horizontal block (0 to 127)";h
20 input " Ending horizontal block (0 to 127)";i
30 input " Starting vertical block (0 to 47)";v
40 input " Ending vertical block (0 to 47)";w
50 cls
60 for x = h to i
70    for y = v to w
80       set(x,y)
90    next y
100 next x
999 goto 999