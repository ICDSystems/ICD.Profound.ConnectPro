# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Changed
 - Fixed source label formatting bug
 - Always set Start Meeting button high so new panel changes are not disruptive
 - Navigate to active calls when a call starts dialing

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
 