[![Build Status](https://ci.appveyor.com/api/projects/status/github/gautema/hcr?branch=master&svg=true)](https://ci.appveyor.com/project/gautema/hcr) 

#HttpClientRecorder

Record HttpClient requests and replay them for fast, deterministic, accurate tests.

Inspired by [vcr](https://github.com/vcr/vcr)

##Getting started
In your test project, create your HttpClient like this: `new HttpClient(new HcrHttpMessageHandler());`


##License 
Copyright 2015 Gaute Magnussen

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
