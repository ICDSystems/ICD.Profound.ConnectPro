# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added
 - Added Touch Free Setting Page.
 - Added Touch Free/ Cancel Meeting Start Prompt.
 - Added OSD Touch Free Notification.

## [14.0.1] - 2020-07-14
### Changed
 - Fixed duplicate sources in event server routing feedback
 - Changed "Out Of Meeting" activity to "Idle"
 - Fixed a bug where meetings would time out after 10 minutes even if a source is routed
 - Sources on the VTC/Zoom share pages are disabled if the device is offline
 - Fixed a bug where base commercial room subscriptions were not being called
 - Fixed a null reference exception in the camera layout subpage

## [14.0.0] - 2020-06-22
### Added
 - Sources now disable when the source device is offline, unless EnableWhileOffline is set on the source
 - Destinations now disable when all destination devices are offline, unless EnableWhileOffline is set on the destination
 - Rooms with one singular source will now launch that source when a meeting is started.
 - Zoom Rooms with one sharable source will now pre-select that source when the left menu sharing button is pressed.
 - Added routing failed feedback to sources, if a device in the routing path is offline when the route is made
 - ConnectPRO sources can be configured to appear in the source selection list
 - Waking the room when it becomes occupied via sensor
 - Added settings pages for configuring Zoom Speaker and Microphone
 - Added basic support for multiple calendar controls

### Changed
 - Changed VTC routing to destination groups to route to individual displays in the group in alternating output order
 - Using new logging context
 - TouchCUE bookings are displayed in local time
 - Fixes to support 11 digit Zoom meeting IDs
 - Faking unrouting feedback to improve UX when switching sources
 - Fixed errors on program stop related to disposed timers
 - Changed Zoom "Call Out" wording to "Phone"
 - ATC call-in number is populated from the device and is no longer configured at the room level
 - Fixed an exception on program stop related to room combine grid lookup
 - Favourite contacts are stored in the new ORM databases
 - Zoom meeting IDs are formatted to a human readable format with dashes

## [13.1.1] - 2020-07-22
### Changed
 - Fixed a bug where meetings would end due to inactivity while a source is routed

## [13.1.0] - 2020-04-29
### Changed
 - Fixed a bug where certain UIs were not being instantiated
 - Changed incoming call pages to use new answer state event args
 - Recent Calls lists now uses ConferenceHistory
 - VTC Contacts presenters use new ConferenceHistory for recent calls

## [13.0.0] - 2020-04-29
### Added
 - Added TouchCUE UI
 - Added CueMotion property on ConnectProTheme for whether to use video or image backgrounds for CUEs
 - Added UI for YKUP USB switcher for automatic switching when Zoom becomes active
 
### Changed
 - Genericized root settings node to allow different roots
 - Don't power off destination when routing the osd if the display is a VibeBoard
 - Change ClockSettingsLeaf to use DateTime instead of TimeSpan to allow for setting the date as well
 - Background settings leaf should be visible if the system has a VibeBoard
 - Moved room dialing features into a new ConnectProDialing class
 
### Removed
 - No longer sorting Zoom contacts by online state

## [12.0.0] - 2020-03-20
### Added
 - Showing ConnectPRO version on the settings passcode screen
 - Added plugin page to the settings menu
 - Added CueName property to ConnectProSource
 - Added breadcrumbs to settings title, showing current location
 - Added floating action list button/popup
 - Added lighting control
 - Volume presenter will now get volume points contextual
 - Showing an offline icon for offline Zoom contacts
 - Added room combine button to the splash page
 - Zoom UI now respects "RecordEnable" and "DialOutEnable" ZoomRoom device settings
 - ZoomSettings now has options to control "RecordEnable", "DialOutEnable", and "MuteMyCameraOnStart"
 - ZoomSettings now has options to map Camera Devices to USB IDs
 - Added ActiveCamera property and event to IConnectProRoom
 - Added event server messages for active camera changes
 - Phsyical TSW lighting button opens the lighting page

### Changed
 - Reworked camera presenters to allow selection and control from a single page
 - Sources on the Cue are distinct based on CueName (if defined) instead of Icon
 - Sources on the Cue now shown using CueName if defined, otherwise source name
 - Changed settings pages to only prompt for PIN code when going to a page that requires it
 - Settings Root and Combine Grid pages don't require login (all other do)
 - Fixed a bug that was causing settings to be saved when the user visits the power settings without making a change
 - Using new volume interfaces for ramping
 - Recent calls lists include unanswered incoming calls
 - When the room goes to sleep routing resets will not power on any displays that are already off.
 - Zoom camera layout buttons will enable/disable based on availability
 - Inverted camera pan buttons
 - ZoomSettings controls "MuteParticipantsOnEntry" on zoom room itself now, and marks settings dirty to save XML

### Removed
 - Privacy Mute floating action is only visible when privacy mute is supported in the current context
 - Changed calls to IdUtils to utilize new enums
 - Using UTC for Times
 - Cameras are muted/unmuted when call state changes
 - Cameras are muted/unmuted when a web conferencing source is routed/unrouted

## [11.4.2] - 2020-03-09
### Changed
 - Fixed a bug where the am/pm toggle wasn't being set correctly for clock settings page

## [11.4.1] - 2020-02-27
### Changed
 - Fixed a bug where combine spaces were not unrouting when growing/shrinking

## [11.4.0] - 2020-02-26
### Changed
 - Moved common UI factory features into Themes project
 - Fixed a NullReferenceException in the room combine presenter

## [11.3.4] - 2020-02-25
### Changed
 - Fixed a bug where the room Focus Source was not being cleared, preventing the same source from being opened again.
 - "Combining Rooms" modal says "Uncombining Rooms" when uncombining

## [11.3.3] - 2020-02-25
### Changed
 - Fixed a bug where the Event Server would not properly re-initialize when changing rooms

## [11.3.2] - 2020-02-25
### Changed
 - Fixed a bug where sources in combine spaces were not showing their combine names

## [11.3.1] - 2020-02-24
### Changed
 - Fixed a bug preventing the Event Server from working correctly in combined rooms

## [11.3.0] - 2020-02-21
### Added
 - Holding the camera home button will store the camera home position

## [11.2.0] - 2020-02-20
### Added
 - The center D-Pad button returns the selected camera to the home position
 - Cameras will return to their home position when starting a video conference

## [11.1.2] - 2020-02-06
### Changed
 - Fixed a bug where combining 2+ rooms with a total of 1 display between them would break the UI
 - Disabling the Advanced Mode button unless there are at least 2 displays

## [11.1.1] - 2020-01-20
### Changed
 - Fixed a URI parsing issue when determining Logo URI on startup
 - Fixed a null reference exception related to quickly pressing the Meet Now button

## [11.1.0] - 2019-12-03
### Added
 - Shure Microphones are available for privacy mute when routing a web conferencing source
 - Showing an error message when Zoom fails to place a phone call

### Changed
 - Privacy mute is turned off when no web conferencing sources are routed and not in a conference
 - All-day bookings are displayed as "All Day" instead of 12:00a to 12:00a

## [11.0.0] - 2019-11-20
### Added
 - Checking in and out of bookings when starting and ending meetings
 - Added UI for Shure MX396 microphones
 - Show a generic alert box when a remote ZoomRoom participant asks us to unmute video
 - Added ConnectProEventServerDevice for feeding meeting, mute, route, etc states to TCP clients
 - Added ZoomRoom settings pages
 - Added favorites support for Zoom contacts
 - Added camera layout features for Zoom
 - Added call lock button for Zoom
 - Added call record button for Zoom
 - Added call record permission button for Zoom participants
 - Added Zoom call-out page
 - Showing an error message when conference recording fails
 - Added "conference override" to ConnectPRO sources, for controlling the visibility of privacy mute and camera features

### Changed
 - Generic Dial Context has been renamed to Traditional Dial Context in the ATC Base Presenter
 - SQLite favorites are stored in the Program<slot>Data/Room<id> directory
 - Fixed a bug where the settings "back" button could leave the settings root!
 - Fixed a bug where room destinations were not being re-cached when the room contents change
 - Camera controls are shown as default camera page instead of active camera
 - Fixed bugs that were preventing the room from completely going to sleep
 - Zoom contacts list correctly updates when contact avatar and online state changes
 - Fixed a bug where meetings would not time out unless the user had routed at least once

### Removed
 - Removed redundant ATC incoming call subpage

## [10.1.3] - 2019-10-25
### Changed
 - Only showing cameras that can be routed to the current VTC

## [10.1.2] - 2019-10-17
### Changed
 - Fixed a bug where DTMF was not available in video calls

## [10.1.1] - 2019-10-10
### Changed
 - Fixed a bug where volume control was not available in a combined space
 - Fixed a bug where the shutdown confirmation page was not hiding the volume page
 - Incoming call answering and dismissal is better synchronized in rooms with multiple panels

## [10.1.0] - 2019-10-08
### Added
 - Showing a confirmation dialog when a remote Zoom participant requests microphone mute
 - Added warming/cooling status labels for displays
 - Room combine dialog is shown on all panels when combining/uncombining rooms
 - Better support for multiple OSDs in a single room
 - Showing a "No Contacts Selected" label when no zoom contacts are selected

### Changed
 - Prevent users from pressing Start My Meeting button when no booking is selected
 - Privacy mute is turned off between calls
 - Using short date format for Start Meeting splash page
 - Generic Alert subpage now supports multiple buttons
 - Panel header text shows the original room for the panel
 - Fixed a bug where display icons would be incorrectly set
 - Combined simple mode display shows "All Displays" as label
 - Fixed various null reference exceptions when combining/uncombining rooms
 - Routing feedback improvements, particularly for processing states
 - Fixed bug with Zoom contact filtering using the new generic keyboard
 - Selected Zoom contacts are removed from the contacts list
 - Better logic for getting the room for a given source, destination or panel 

## [10.0.0] - 2019-09-16
### Added
 - Added settings page for changing clock time and 12/24 hour mode
 - ClockAudio CCRM4000 will extend and retract based on call state
 - Added a generic keyboard presenter and views
 - The user is presented with a keyboard when a zoom meeting asks for a password

### Changed
 - Only updating relevant displays presenters halves routing feedback time
 - CUE Pages will now properly be hidden in rooms without a scheduler
 - Complete overhaul of settings pages
 - Performance improvement for routing in single display rooms
 - All keyboard usages have been changed to the generic keyboard
 - Updated IPowerDeviceControls consumers to use PowerState
 - Updated Volume Controls to use ControlAvaliable instead of power state

## [9.1.0] - 2019-08-27
### Added
 - CUE background mode controlled by ConnectPRO with CueBackground config element on ConnectProTheme
 - Multiple camera support in Zoom and other wtc/vtc.
 - Added camera active and camera control presenters.
 - Added camera active selection and camera control subpage views.

## [9.0.0] - 2019-08-15
### Added
 - Added CUE Conference page to show instead of routing the Zoom Room until the device is in a meeting
 - DateTimes on the UI and CUE are formatted using the current culture info
 - Zoom contact list filter indication
 - Added "Host" and "Self" tags to Zoom participants
 - Added OSD UnRouting on Room Uncombine
 - Added loading spinner when combining rooms
 - Added Dark Blue Room labels to reperesent the current combined space
 - Cull video sources if they can not be routed to any display in the room
 - Displays enter grey mode when selected source can not be routed
 - Unroute audio from audio destinations when routing audio that can not reach those destinations
 - Added VTC Control mirroring
 - Added camera list to the camera control presenter
 - Added automatic generation of conference points for sources with conferencing controls that don't have conference points
 - Added page on UI for 3+ displays in a room not in combine mode
 - Added Meeting Timeouts, if the room is inactive (i.e. no sources routed, no active conferences, no focus sources) for 10 minutes the meeting is ended.
 
### Changed
 - Significant overhaul of routing, better performance for systems with multiple panels
 - Zoom contact list will clear its filter when the page changes
 - Sort Zoom participants list (host first, self second, sort by name after)
 - Show current contact filter when opening keyboard to filter contacts
 - Clear selected contacts when leaving zoom contacts page
 - CUE conference page shows Zoom specific information
 - Can no longer open partitions that are not adjacent to the current space
 - Can only select partitions that are adjacent to the current space, orphaned partitions are then selected to be closed
 - Only show display speaker button if the routed source can be routed to at least 1 room audio source
 - User's will be kept in advanced mode if no sources can be routed to all displays
 - The default camera to route video to is now selected on program start istead of on camera select view
 - Resetting routing back to OSD when putting the room to sleep
 - Loading spinners now timeout if given a specified timeout parameter
 - The source subpage reference list now scrolls back to the beginning when the meeting state changes
 - The display subpage reference list now scrolls back to the beginning when the combined advanced mode state changes
 - Better booking icon method for CUE
 - Refresh ATC view when conference state changes
 - VTC view is updated when there is an active conference
 - Changed the time in CUE header to localized string
 - Route Summary Menu now sorts by room then by destination
 - The source selection timeout timer is now reset upon routing a source to a destination
 - When the end meeting button is pressed and the system is supposed to be asleep, fully powers down the room
 - The Wake Schedule Sleep Time will no longer end active conferences or local presentations

## [8.1.1] - 2019-05-07
### Changed
 - Fixed null reference in ATC presenter
 - Fixed enumeration exception when ending meeting

## [8.1.0] - 2019-02-20
### Changed
 - Incremented information version to 1.2
 - Fixed null reference in GenericAlertPresenter
 - Fixed VTC for conferencing refactor
 - Refresh available bookings when current booking expires or next booking starts

## [8.0.0] - 2019-01-14
### Added
 - Added web conferencing support through Zoom

### Changed
 - Updating namespaces for port configuration features

## [7.6.0] - 2019-10-01
### Changed
 - ConnectProRoom inherits from AbstractCommercialRoom

## [7.5.0] - 2019-07-26
### Added
 - Supporting Shure MXWAPT devices

## [7.4.0] - 2019-06-07
### Changed
 - UI and UI Factory abstractions and interfaces moved into Themes project
 - Panels and OSD only come online once the core and program have completed initializing

## [7.3.1] - 2019-01-21
### Changed
 - Hide speaker buttons if the room has no audio destinations

## [7.3.0] - 2019-01-21
### Changed
 - Immediately showing processing sources as routed to the displays in the UI
 - ActiveSource renamed to SelectedSource for clarity

## [7.2.2] - 2019-01-14
### Changed
 - Clearing processing sources when ending a meeting

## [7.2.1] - 2019-01-10
### Changed
 - Don't show both caller-info name and number when they are identical 

## [7.2.0] - 2018-11-21
### Added
 - Added originator id attribute to room calendar settings

### Changed
 - Routing no longer assumes there is a single VTC device.
 - UI and OSD are refactored to use common MVP framework
 - Fixed null refs when stopping the program

## [7.1.0] - 2018-10-30
### Changed
 - Increased MPC3 volume ramp speed
 - Holding power button on the MPC3 shuts down the system
 - Disable MPC3 volume buttons when the display is powered off

## [7.0.0] - 2018-10-18
### Added
 - One Button To Push and Calendaring features

### Changed
 - Improved performance when selecting bookings
 - Disable ATC clear and dial buttons while in a call
 - OSD calendaring overhaul

## [6.1.2] - 2018-10-04
### Changed
 - Unroute VTC presentation source when ending presentation
 - Fixing bad VTC share feedback
 - Unselecting VTC share source on subpage visibility change
 - Fixed bug where ShureMXA LED would temporarily flash white

## [6.1.1] - 2018-09-27
### Changed
 - Better fix for audio routing in specific async switcher situations
 - Fixed bug where incoming call text was not updating for subsequent calls

## [6.1.0] - 2018-09-25
### Changed
 - Fixed audio routing in specific async switcher situations
 - Fixed bug where VTC would not unroute after confirming end call
 - Fixed bugs where a source could be routed multiple times to the same destination
 - Fixed references to new volume controls
 - Volume ramping is now positional rather than absolute, resulting in more responsive routing
 - Mic LED no longer has initial white flash
 - Return to VTC subpages after a call ends instead of immediately unrouting
 - Volume down now unmutes as well
 - Resetting routing when occupancy is detected

## [6.0.0] - 2018-09-14
### Changed
 - Considerable performance improvements, bug fixes, etc

## [5.0.0] - 2018-07-19
### Added
 - Settings pages
 - Automatic wake/sleep scheduling
 - Added VTC page for polycom remote control 

### Changed
 - Fixed source label formatting bug
 - Always set Start Meeting button high so new panel changes are not disruptive
 - Navigate to active calls when a call starts dialing
 - Improved workflows for rooms without OSD.
 - UserInterfaces are only created for originator(s) in a room

## [4.1.1] - 2018-07-02
### Changed
 - Updated routing to include audio destinations that have a video flag in addition to the audio flag

## [4.1.0] - 2018-06-19
### Changed
 - Removed dependency on Cisco

## [4.0.1] - 2018-06-04
### Changed
 - Routing performance optimizations
 - No longer setting phonebook type on directory browser

## [4.0.0] - 2018-05-24
### Added
 - ATC implementation

### Changed
 - Default web conferencing path to processor address
 - Volume button goes red when volume is muted
 - Allowing user to route source to 2 destinations without reselecting the source
 - User can switch presentation source without ending presentation and starting again.
 - VTC Camera close button closes the camera page instead of ending conference

## [3.2.0] - 2018-05-09
### Changed
 - Setting the selection state of VTC subpage buttons based on current active subpage
 - Using codec input types to determine presentation routing endpoint
 - Fixing display issues in the VTC Camera presenter
 - Routing presentation audio and video seperately
 - Fixing issues with loading configs with UTF8-BOM encoding

## [3.1.0] - 2018-05-03
### Changed
 - When routing a path ensure we set the destination input and power state regardless of device type

## [3.0.0] - 2018-04-27
### Added
 - Added camera preset saving and recalling
 - Manual dialing via keyboard
 - Adding ConnectProVolumePoint

### Changed
 - Hiding favourites icon on recent calls
 - Pressing the favourite icon on a favourited contact removes the contact from favourites
 - Incoming call subpage now updates to show latest information for caller
 - Using VolumePoints from originators collection
 - Using DialingPlan from abstract room settings
 - Hangup button goes to the active call list
 - Sharing source also routes audio to VTC

## [2.0.0] - 2018-04-23
### Changed
 - Support for sources/destinations with multiple addresses
 