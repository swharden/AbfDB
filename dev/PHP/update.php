<?php

$updateDatabaseCommand = 'D:\X_Drive\Software\AbfDB\Builder\AbfDB.exe update "D:\X_Drive\Data" "D:\X_Drive\Lab Documents\network\locked\abfdb\abfs.db"';

function updateDatabaseAndEchoOutput($updateDatabaseCommand)
{
  flush();
  ob_flush();

  $output = null;
  $returnValue = null;
  exec($updateDatabaseCommand, $output, $returnValue);

  foreach ($output as $line)
    echo ("<div><code>$line</code></div>");

  if ($returnValue)
    echo ('<div class="alert alert-warning my-5">' .
      '<h4 class="alert-heading">ERROR</h4>Execution failed.<br>' .
      'Check C:\Apache24\logs\error.log</div>');
}
?>

<!doctype html>
<html lang="en">

<head>
  <meta charset="utf-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link href="/css/bootstrap.min.css" rel="stylesheet">
  <script src="/js/bootstrap.bundle.min.js"></script>
  <title>Update ABF Database</title>
</head>

<body class="my-5">
  <div class="container">

    <div class="display-5 d-inline-block" id='statusMessage'>
      Updating Database...
    </div>
    <div class="spinner-border text-primary " role="status" id="spinner">
      <span class="visually-hidden">Loading...</span>
    </div>
    <div class="mb-3"><code><?php echo $updateDatabaseCommand; ?></code></div>

    <?php updateDatabaseAndEchoOutput($updateDatabaseCommand); ?>

    <script type="text/javascript">
      document.getElementById('spinner').style.visibility = 'hidden';
      document.getElementById('statusMessage').innerText = 'Database Updated.';
    </script>

  </div>
</body>

</html>