#Copy-Item ./DockerTools/SampleWithBuild.DockerFile ./sample/WKeSampleApp
Docker build -f ./DockerTools/SampleWithBuild.DockerFile -t wke/sampleapp .