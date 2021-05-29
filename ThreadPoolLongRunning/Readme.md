# Testing thread scheduling behavior with and without the LongRunning flag.
This program illustrates the behavior of the thread scheduler when trying to spin up
many long-running tasks.

It shows how not using the LongRunning flag causes task creation to be significantly delayed as injection of new
ThreadPool threads happens infrequently.