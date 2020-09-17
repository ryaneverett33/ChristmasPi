CC=gcc
CFLAGS=-L
OUT=./

WRAPPERSDIR=../wrappers/

testpidexists: testpidexists.c
	$(MAKE) -C ../wrappers/ pidexists
	cp ../wrappers/libpidexists.so $(OUT)libpidexists.so
	$(CC) $(CFLAGS). -lpidexists testpidexists.c -o $(OUT)testpidexists

clean-testpidexists: 
	-rm $(OUT)testpidexists
	-rm $(OUT)libpidexists.so

clean: clean-testpidexists

all: testpidexists

.PHONY: clean all