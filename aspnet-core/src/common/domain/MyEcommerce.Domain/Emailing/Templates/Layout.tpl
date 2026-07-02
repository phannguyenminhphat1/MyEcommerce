<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />

    <style>
        body{
            margin:0;
            padding:0;
            background:#f4f4f4;
            font-family:Arial,Helvetica,sans-serif;
        }

        .wrapper{
            width:100%;
            padding:40px 0;
            background:#f4f4f4;
        }

        .container{
            width:600px;
            margin:auto;
            background:white;
            border-radius:10px;
            overflow:hidden;
            box-shadow:0 4px 20px rgba(0,0,0,.08);
        }

        .header{
            background:#2563eb;
            color:white;
            text-align:center;
            padding:28px;
            font-size:28px;
            font-weight:bold;
        }

        .content{
            padding:40px;
            color:#444;
            line-height:1.7;
            font-size:16px;
        }

        .footer{
            background:#f7f7f7;
            text-align:center;
            color:#888;
            padding:20px;
            font-size:13px;
        }

        a{
            color:#2563eb;
            text-decoration:none;
        }

        .button{
            display:inline-block;
            padding:12px 22px;
            background:#2563eb;
            color:white !important;
            border-radius:6px;
            text-decoration:none;
            margin-top:20px;
        }
    </style>

</head>

<body>

<div class="wrapper">

    <div class="container">

        <div class="header">
            🛒 MyEcommerce
        </div>

        <div class="content">
            {{content}}
        </div>

        <div class="footer">
            © 2026 MyEcommerce<br/>
            Thank you for shopping with us.
        </div>

    </div>

</div>

</body>
</html>