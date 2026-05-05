


## Decision Log (slight deviations from requirements)

1. Instructions say "User receives a visual cue when a value is greater than 9000". Is it strictly greater than? From UX perspective it seems better to have  > Abs(9000) instead to keep UI neat. Going with absolute value for now 

2. Instructions say "whole numbers and commas" are only allowed chars, decided to allow whitespace as well which gets trimmed during validation, for better UX

3. 