CC=gcc
CFLAGS=-shared
OUT=./

pidexists: pidexists.c
	$(CC) $(CFLAGS) pidexists.c -o $(OUT)libpidexists.so 

clean-pidexists: 
	-rm libpidexists.so

clean: clean-pidexists

all: pidexists

.PHONY: clean all