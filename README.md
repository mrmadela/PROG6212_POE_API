# PROG6212_POE_API
Application Programming Interface

BUSINESS ROLES 

Authentication & Users 

Every user must have a unique email address across the system. 

A user's role (Organiser or Participant) is fixed at registration and determines which profile record is linked to that account. 

Each user account links to exactly one profile record — an Organiser account never has a Participant profile, and vice versa. 

Login credentials (email, password) live only on the users record — they are never duplicated on the organiser or participant profile. 

Organisers 5. Only Organisers may create, update, or delete events. 6. Only the Organiser who created an event (the "owner") may update it, delete it, or manage its categories — other Organisers cannot. 

Participants 7. Any logged-in user (either role) may browse events and view available categories. 8. A Participant may only view or update their own profile, enrolments, and results — never another participant's. 

Events 9. Every event must capture a name, description, date, location, distance, and type (Run, Walk, or Cycle). 10. An event cannot be deleted while it still has categories or enrolments linked to it. 

Categories 11. Every category belongs to exactly one event. 12. A category must be either age-based or distance-based — never both, and never neither. 13. A category cannot be deleted while participants are enrolled in it. 

Event Enrolments 14. A participant may enrol in a given category only once (no duplicate enrolments for the same participant + category). 15. Every enrolment records the participant, the event, and the category together — the event is stored directly, not just inferred through the category. 16. A participant may cancel their own enrolment, but not once a result has already been captured against it. 17. Only the Organiser who owns the event may view the full list of enrolments for it. 

Results 18. A result can only be captured for an enrolment that already exists. 19. Each enrolment can have at most one result. 20. Only the Organiser who owns the related event may capture or update a result. 21. A participant may only view their own results, never another participant's. 
