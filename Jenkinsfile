
pipeline {
    agent { dockerfile true }
    environment {
        SONARQUBE_PROJECT_KEY = "vtb-utils"
        SONARQUBE_TOKEN = credentials('SONARQUBE_TOKEN');
        SONARQUBE_URL = credentials('SONARQUBE_URL');
        BRANCH_NAME = "${env.BRANCH_NAME}";
    }

    stages {
        stage('SCA Setup') {
            steps {
                sh '''\
                    dotnet sonarscanner begin \
                        /k:"${SONARQUBE_PROJECT_KEY}" \
                        /d:sonar.login="${SONARQUBE_TOKEN}" \
                        /d:sonar.host.url="${SONARQUBE_URL}" \
                        /d:sonar.coverage.exclusions="*.Tests/" \
                        /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
                    '''
            }
        }
        stage('Build') {
            steps {
                sh 'dotnet build -p:Version=${BUILD_NUMBER}'
            }
        }
        stage('Run All Tests') {
            steps {
                sh '''\
                    dotnet test \
                        --no-build \
                        /p:CollectCoverage=true \
                        /p:CoverletOutputFormat="opencover" \
                        /p:CoverletOutput=./
                '''
            }
        }
        stage('SonarScanner Wrap Up') {
            steps {
                sh 'dotnet sonarscanner end /d:sonar.login="${SONARQUBE_TOKEN}"'
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: 'out/*', fingerprint: true
            publishCoverage adapters: 
                [opencoverAdapter(mergeToOneReport: true, path: 'vtb.Utils.Tests/coverage.opencover.xml')], 
                sourceFileResolver: sourceFiles('NEVER_STORE')
        }
    }
}