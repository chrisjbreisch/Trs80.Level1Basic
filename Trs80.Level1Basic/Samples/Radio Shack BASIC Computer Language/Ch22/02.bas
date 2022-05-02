10 cls
20 print at 407, "H  M   S"
30 for h = 0 to 23
40    for m = 0 to 59
50       for s = 0 to 59
60          print at 470, h;":";m;":";s;" "
70          for n = 1 to 2000000: next n
80       next s
90    next m
100 next h