[CmdletBinding()]
Param(
    [Parameter(Mandatory=$False)]
    [string]$tcSlnDir
)

if($tcSlnDir -ne "") {
	$tcItem = Get-Item $tcSlnDir
} else {
	$tcItem = Get-Item ".\"
}

# Clear out all of the views that currently exist in the Views directory

# Check to make sure the directory exists first.  If not create it
if(!(test-path "$tcItem\TestController\Views")) {
	new-item -path "$tcItem\TestController\Views" -ItemType Directory 
} else {
	remove-item "$tcItem\TestController\Views\*" -recurse
}

# Get a list of all of the views folders excluding testcontroller and pxwebapi
$pxDir = $tcItem.Parent.FullName
$viewDirectories = gci "$pxDir" -recurse | where {$_.Name -like "Views" -and $_.FullName -notlike "*TestController*" -and $_.FullName -notlike "*PXWebAPI*"}

# Copy the views into the test controller's view directory
foreach( $v in $viewDirectories) {
	$parent = $v.Parent.Name
	$path = $v.FullName
	write-host "Copying $parent"
	cpi "$path" "$tcItem\TestController\Views\$parent" -recurse
}