CC=gcc
CFLAGS=-L
OUT=./

WRAPPERSDIR=./

testpidexists: testpidexists.c
	$(MAKE) -C ../wrappers/ pidexists
	cp ../wrappers/libpidexists.so $(OUT)libpidexists.so
	$(CC) $(CFLAGS). -lpidexists testpidexists.c -o $(OUT)testpidexists

clean-testpidexists: 
	-rm $(OUT)libpidexists.so
	-rm $(OUT)testpidexists

clean: clean-testpidexists

all: testpidexists

.PHONY: clean all