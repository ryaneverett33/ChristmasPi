#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>
#include <unistd.h>
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/in.h>
#include <arpa/inet.h>
#include <jansson.h>

int main(int argc, char *arg_v[]) {
    char *buffer = "[{'login':1},{'machineid':123}]";
    printf("Starting handle. \n");
    json_t *root;
    json_error_t jerror;
    root = json_loads(buffer, 0, &jerror);
    if(!root)
    {
        fprintf(stderr, "error: on line %d: %s\n", jerror.line, jerror.text);
    }
    json_t *client = json_array_get(root,0);
    json_t *login = json_object_get(client,"login");
    if(!json_is_array(root))
    {
        puts("root is not an object");
    }
    if(!json_is_integer(login))
    {
        puts("its not a string");
    }
    json_t *machineid = json_object_get(client,"machineid");
    if(!json_is_integer(machineid))
    {
        puts("Machineid is not an integer");
    }
    int i_login = json_integer_value(login);
    int i_machineid = json_integer_value(machineid);
    printf("login: %i",i_login);
    printf("machineid: %i \n",i_machineid);
    printf("testtyyy \n");
}
