
$baseDir = $args[0]
$rename = $args[1]

if(Test-Path "$baseDir\Web.config.backup") {
	Remove-Item "$baseDir\Web.config.backup"
}

Rename-Item "$baseDir\Web.config" Web.config.backup
Rename-Item "$baseDir\$rename" Web.config