#include <stdlib.h>
#include <stdio.h>
#include <string.h>

int main(int argc, char* argv[]){
    char msg[1024];
	int msglen, lenwr, lenrd;

	do
	{
		//skriver till stdout
		printf("PlanetCNC: ");
		//läser från stdin
		fgets(msg, 1024, stdin);
		msglen = strnlen(msg, 1024);

		if (msglen > 1){

			printf("%s\n", msg); 
	    }
			

	} while (msglen > 1);


	return 0;
}