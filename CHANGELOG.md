# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [Unreleased]

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
 