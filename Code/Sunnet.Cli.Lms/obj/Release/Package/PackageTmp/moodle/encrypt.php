<?php 

class DES 
{ 
    var $key; 
    var $iv; 
    public function DES($key='.!e@0Na&') 
    { 
        $key = substr($key, 0, 8); 
        $this->key = $key; 
        $this->iv = $key; 
    } 
    public function encrypt($str) 
    { 
        $size = mcrypt_get_block_size ( MCRYPT_DES, MCRYPT_MODE_CBC ); 
        $str = $this->pkcs5Pad ( $str, $size ); 
        return strtoupper(bin2hex(mcrypt_cbc(MCRYPT_DES, $this->key, $str, MCRYPT_ENCRYPT, $this->iv ))); 
    } 
    public function decrypt($str) 
    { 
        $str = $this->hex2bin(strtolower($str));
        $str = mcrypt_cbc( MCRYPT_DES, $this->key, $str, MCRYPT_DECRYPT, $this->iv ); 
        $str = $this->pkcs5Unpad( $str ); 
        return $str; 
    } 
    
    function hex2bin($hexData) {
        $binData = "";
        for($i = 0; $i < strlen ( $hexData ); $i += 2) {
            $binData .= chr ( hexdec ( substr ( $hexData, $i, 2 ) ) );
        }
        return $binData;
    }
    function pkcs5Pad($text, $blocksize) 
    { 
        $pad = $blocksize - (strlen ( $text ) % $blocksize); 
        return $text . str_repeat ( chr ( $pad ), $pad ); 
    }
    function pkcs5Unpad($text) 
    { 
        $pad = ord ( $text {strlen ( $text ) - 1} ); 
        if ($pad > strlen ( $text )) 
        return false; 
        if (strspn ( $text, chr ( $pad ), strlen ( $text ) - $pad ) != $pad) 
        return false; 
        return substr ( $text, 0, - 1 * $pad ); 
    } 
} 
$des= new DES();
echo $des->encrypt("CLIEngageToken");
//echo $des->decrypt("9772793214ED714F3082A04E18682F58");
?>