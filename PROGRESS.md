# Yo
## I created the basic DB schema for our app. Stations and buses are both very simple, they don't need to hold much
information in the database. A bus in the DB is a bus line, so not an actual instance of a bus! Actual buses will send
real-time information that's used for real-time computations, so no need to put that in the database. We can now
define the routes that each bus line will take, `position` referring to the order they take (e.g. Station Groningen
first, then Groningen Noord second).
