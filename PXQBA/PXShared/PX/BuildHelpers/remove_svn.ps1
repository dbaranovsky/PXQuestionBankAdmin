$targetDir = $args[0]

gci $targetDir -include .svn -Recurse -Force | Remove-Item -Recurse -Force