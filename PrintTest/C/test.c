#include <windows.h>
#include <stdio.h>

int main() {
    LARGE_INTEGER startTime, endTime, frequency;
    QueryPerformanceFrequency(&frequency);
    
    QueryPerformanceCounter(&startTime);

    for (int i = 0; i < 1000; i++) {
        printf("Iteration %d\n", i);
    }

    QueryPerformanceCounter(&endTime);

    double elapsedMilliseconds = ((double)(endTime.QuadPart - startTime.QuadPart) / frequency.QuadPart) * 1000.0;
    
    printf("Time elapsed: %.3f milliseconds\n", elapsedMilliseconds);
    
    return 0;
}
