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
int portnum = 8080;
struct packet_info {
    bool isLogin;//Not a PUSH, does get returns
    bool isLogout;//Not a PUSH, does get returns
    bool isPush;
    bool isReturn; //Not supporting, declaring anyway
    int machineid;
    char *password;
};
struct active_user {
    int machineid;
    char *password;
};


void doLogout(int n, char *buffer) {

}
void doLogin(int n, char *buffer) {

}
void doReturn(int n, char *buffer) {
    struct packet_info packet_info;
    if(packet_info.isLogin == true)
    {

    }
    else if(packet_info.isLogout == true){

    }
}
void handle(int n, char *buffer) {
    printf("Starting handle. \n");
    printf("input: %s",buffer);
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
    client = json_array_get(root,1);
    json_t *machineid = json_object_get(client,"machineid");
    if(!json_is_integer(machineid))
    {
        puts("Machineid is not an integer");
    }
    client = json_array_get(root,2);
    json_t *password = json_object_get(client, "password");
    if(!json_is_string(password))
    {
        puts("Password is not a string");
    }
    json_t *branch1,*branch2,*branch3,*branch4,*branch5,*branch6,*star,*train,*animation;
    client=json_array_get(root,3);
    branch1 = json_object_get(client,"branch1");
    client=json_array_get(root,4);
    branch2 = json_object_get(client,"branch2");
    client=json_array_get(root,5);
    branch3 = json_object_get(client,"branch3");
    client=json_array_get(root,6);
    branch4 = json_object_get(client,"branch4");
    client=json_array_get(root,7);
    branch5 = json_object_get(client,"branch5");
    client=json_array_get(root,8);
    branch6 = json_object_get(client,"branch6");
    client=json_array_get(root,9);
    star = json_object_get(client,"star");
    client=json_array_get(root,10);
    train = json_object_get(client,"train");
    client=json_array_get(root,11);
    animation = json_object_get(client,"animation");
    int i_login = json_integer_value(login);
    int i_machineid = json_integer_value(machineid);
    char *s_password = json_string_value(password);
    int i_branch1 = json_integer_value(branch1);
    int i_branch2 = json_integer_value(branch2);
    int i_branch3 = json_integer_value(branch3);
    int i_branch4 = json_integer_value(branch4);
    int i_branch5 = json_integer_value(branch5);
    int i_branch6 = json_integer_value(branch6);
    int i_star = json_integer_value(star);
    int i_train = json_integer_value(train);
    char *s_animation = json_string_value(animation);
    printf("login: %i \n",i_login);
    printf("machineid: %i \n",i_machineid);
    printf("password: %s \n",s_password);
    printf("branch1: %i \n",i_branch1);
    printf("branch2: %i \n",i_branch2);
    printf("branch3: %i \n",i_branch3);
    printf("branch4: %i \n",i_branch4);
    printf("branch5: %i \n",i_branch5);
    printf("branch6: %i \n",i_branch6);
    printf("star: %i \n",i_star);
    printf("train: %i \n",i_train);
    printf("animation: %s \n",s_animation);
}
void error(const char *msg) {
    perror(msg);
    exit(1);
}
int main() {
    //Setup Server
    //Create a new socket to bind
    char buffer[256];
    int sockfd, newsockfd;
    socklen_t clilen;

    struct sockaddr_in serv_addr, cli_addr;
    int n;
    struct hostent *server;
    char ipstr[INET6_ADDRSTRLEN];
    char* address;
    sockfd = socket(AF_INET, SOCK_STREAM, 0);
    if (sockfd < 0)
    error("ERROR opening socket");
    bzero((char *) &serv_addr, sizeof(serv_addr));
    serv_addr.sin_family = AF_INET;
    serv_addr.sin_addr.s_addr = INADDR_ANY;
    serv_addr.sin_port = htons(portnum);
    //Check bind
    if (bind(sockfd, (struct sockaddr *) &serv_addr, sizeof(serv_addr)) < 0) {
        error("ERROR on binding");
    }
    //Server Loop
    do {
    //Listen for connections on socket "sockfd" with a maximum queue of 5 connections (arg 5)
        listen(sockfd,5);
        clilen = sizeof(cli_addr);
        newsockfd = accept(sockfd,
            (struct sockaddr *) &cli_addr,
            &clilen);
        if (newsockfd < 0)
            error("ERROR on accept");
        bzero(buffer,256);
        n = read(newsockfd,buffer,255);
        getpeername(n, &cli_addr, &clilen);
        char* cli_address = inet_ntoa(cli_addr.sin_addr);
        printf("Peer's IP address is: %s\n", cli_address);
        printf("Here is the message: %s\n",buffer);
        handle(n,buffer);
        close(newsockfd);
         }
    while(1==1);
    return 1;
}
