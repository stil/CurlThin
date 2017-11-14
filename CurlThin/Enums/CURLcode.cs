using System.Diagnostics.CodeAnalysis;

namespace CurlThin.Enums
{
    /// <summary>
    ///     Reference: https://github.com/curl/curl/blob/master/include/curl/curl.h
    /// </summary>
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public enum CURLcode : uint
    {
        OK = 0,

        /// <summary>1</summary>
        UNSUPPORTED_PROTOCOL,

        /// <summary>2</summary>
        FAILED_INIT,

        /// <summary>3</summary>
        URL_MALFORMAT,

        /// <summary>4 - [was obsoleted in August 2007 for 7.17.0, reused in April 2011 for 7.21.5]</summary>
        NOT_BUILT_IN,

        /// <summary>5</summary>
        COULDNT_RESOLVE_PROXY,

        /// <summary>6</summary>
        COULDNT_RESOLVE_HOST,

        /// <summary>7</summary>
        COULDNT_CONNECT,

        /// <summary>8</summary>
        WEIRD_SERVER_REPLY,

        /// <summary>9 a service was denied by the server due to lack of access - when login fails this is not returned.</summary>
        REMOTE_ACCESS_DENIED,

        /// <summary>10 - [was obsoleted in April 2006 for 7.15.4, reused in Dec 2011 for 7.24.0]</summary>
        FTP_ACCEPT_FAILED,

        /// <summary>11</summary>
        FTP_WEIRD_PASS_REPLY,

        /// <summary>
        ///     12 - timeout occurred accepting server
        ///     [was obsoleted in August 2007 for 7.17.0, reused in Dec 2011 for 7.24.0]
        /// </summary>
        FTP_ACCEPT_TIMEOUT,

        /// <summary>13</summary>
        FTP_WEIRD_PASV_REPLY,

        /// <summary>14</summary>
        FTP_WEIRD_227_FORMAT,

        /// <summary>15</summary>
        FTP_CANT_GET_HOST,

        /// <summary>
        ///     16 - A problem in the http2 framing layer.
        ///     [was obsoleted in August 2007 for 7.17.0, reused in July 2014 for 7.38.0]
        /// </summary>
        HTTP2,

        /// <summary>17</summary>
        FTP_COULDNT_SET_TYPE,

        /// <summary>18</summary>
        PARTIAL_FILE,

        /// <summary>19</summary>
        FTP_COULDNT_RETR_FILE,

        /// <summary>20 - NOT USED</summary>
        OBSOLETE20,

        /// <summary>21 - quote command failure</summary>
        QUOTE_ERROR,

        /// <summary>22</summary>
        HTTP_RETURNED_ERROR,

        /// <summary>23</summary>
        WRITE_ERROR,

        /// <summary>24 - NOT USED</summary>
        OBSOLETE24,

        /// <summary>25 - failed upload "command"</summary>
        UPLOAD_FAILED,

        /// <summary>26 - couldn't open/read from file</summary>
        READ_ERROR,

        /// <summary>
        ///     27 - Note: OUT_OF_MEMORY may sometimes indicate a conversion error instead of a memory allocation error if
        ///     CURL_DOES_CONVERSIONS is defined
        /// </summary>
        OUT_OF_MEMORY,

        /// <summary>28 - the timeout time was reached</summary>
        OPERATION_TIMEDOUT,

        /// <summary>29 - NOT USED</summary>
        OBSOLETE29,

        /// <summary>30 - FTP PORT operation failed</summary>
        FTP_PORT_FAILED,

        /// <summary>31 - the REST command failed</summary>
        FTP_COULDNT_USE_REST,

        /// <summary>32 - NOT USED</summary>
        OBSOLETE32,

        /// <summary>33 - RANGE "command" didn't work</summary>
        RANGE_ERROR,

        /// <summary>34</summary>
        HTTP_POST_ERROR,

        /// <summary>35 - wrong when connecting with SSL</summary>
        SSL_CONNECT_ERROR,

        /// <summary>36 - couldn't resume download</summary>
        BAD_DOWNLOAD_RESUME,

        /// <summary>37</summary>
        FILE_COULDNT_READ_FILE,

        /// <summary>38</summary>
        LDAP_CANNOT_BIND,

        /// <summary>39</summary>
        LDAP_SEARCH_FAILED,

        /// <summary>40 - NOT USED</summary>
        OBSOLETE40,

        /// <summary>41 - NOT USED starting with 7.53.0</summary>
        FUNCTION_NOT_FOUND,

        /// <summary>42</summary>
        ABORTED_BY_CALLBACK,

        /// <summary>43</summary>
        BAD_FUNCTION_ARGUMENT,

        /// <summary>44 - NOT USED</summary>
        OBSOLETE44,

        /// <summary>45 - CURLOPT_INTERFACE failed</summary>
        INTERFACE_FAILED,

        /// <summary>46 - NOT USED</summary>
        OBSOLETE46,

        /// <summary>47 - catch endless re-direct loops</summary>
        TOO_MANY_REDIRECTS,

        /// <summary>48 - User specified an unknown option</summary>
        UNKNOWN_OPTION,

        /// <summary>49 - Malformed telnet option</summary>
        TELNET_OPTION_SYNTAX,

        /// <summary>50 - NOT USED</summary>
        OBSOLETE50,

        /// <summary>51 - peer's certificate or fingerprint wasn't verified fine</summary>
        PEER_FAILED_VERIFICATION,

        /// <summary>52 - when this is a specific error</summary>
        GOT_NOTHING,

        /// <summary>53 - SSL crypto engine not found</summary>
        SSL_ENGINE_NOTFOUND,

        /// <summary>54 - can not set SSL crypto engine as default</summary>
        SSL_ENGINE_SETFAILED,

        /// <summary>55 - failed sending network data</summary>
        SEND_ERROR,

        /// <summary>56 - failure in receiving network data</summary>
        RECV_ERROR,

        /// <summary>57 - NOT IN USE</summary>
        OBSOLETE57,

        /// <summary>58 - problem with the local certificate</summary>
        SSL_CERTPROBLEM,

        /// <summary>59 - couldn't use specified cipher</summary>
        SSL_CIPHER,

        /// <summary>60 - problem with the CA cert (path?)</summary>
        SSL_CACERT,

        /// <summary>61 - Unrecognized/bad encoding</summary>
        BAD_CONTENT_ENCODING,

        /// <summary>62 - Invalid LDAP URL</summary>
        LDAP_INVALID_URL,

        /// <summary>63 - Maximum file size exceeded</summary>
        FILESIZE_EXCEEDED,

        /// <summary>64 - Requested FTP SSL level failed</summary>
        USE_SSL_FAILED,

        /// <summary>65 - Sending the data requires a rewind that failed</summary>
        SEND_FAIL_REWIND,

        /// <summary>66 - failed to initialise ENGINE</summary>
        SSL_ENGINE_INITFAILED,

        /// <summary>67 - user, password or similar was not accepted and we failed to login</summary>
        LOGIN_DENIED,

        /// <summary>68 - file not found on server</summary>
        TFTP_NOTFOUND,

        /// <summary>69 - permission problem on server</summary>
        TFTP_PERM,

        /// <summary>70 - out of disk space on server</summary>
        REMOTE_DISK_FULL,

        /// <summary>71 - Illegal TFTP operation</summary>
        TFTP_ILLEGAL,

        /// <summary>72 - Unknown transfer ID</summary>
        TFTP_UNKNOWNID,

        /// <summary>73 - File already exists</summary>
        REMOTE_FILE_EXISTS,

        /// <summary>74 - No such user</summary>
        TFTP_NOSUCHUSER,

        /// <summary>75 - conversion failed</summary>
        CONV_FAILED,

        /// <summary>
        ///     76 - caller must register conversion callbacks using curl_easy_setopt options
        ///     CURLOPT_CONV_FROM_NETWORK_FUNCTION,
        ///     CURLOPT_CONV_TO_NETWORK_FUNCTION, and
        ///     CURLOPT_CONV_FROM_UTF8_FUNCTION
        /// </summary>
        CONV_REQD,

        /// <summary>77 - could not load CACERT file, missing or wrong format</summary>
        SSL_CACERT_BADFILE,

        /// <summary>78 - remote file not found</summary>
        REMOTE_FILE_NOT_FOUND,

        /// <summary>
        ///     79 - error from the SSH layer, somewhat generic so the error message will be of interest when this has
        ///     happened
        /// </summary>
        SSH,

        /// <summary>80 - Failed to shut down the SSL connection</summary>
        SSL_SHUTDOWN_FAILED,

        /// <summary>81 - socket is not ready for send/recv, wait till it's ready and try again (Added in 7.18.2)</summary>
        AGAIN,

        /// <summary>82 - could not load CRL file, missing or wrong format (Added in 7.19.0)</summary>
        SSL_CRL_BADFILE,

        /// <summary>83 - Issuer check failed. (Added in 7.19.0)</summary>
        SSL_ISSUER_ERROR,

        /// <summary>84 - a PRET command failed</summary>
        FTP_PRET_FAILED,

        /// <summary>85 - mismatch of RTSP CSeq numbers</summary>
        RTSP_CSEQ_ERROR,

        /// <summary> 86 - mismatch of RTSP Session Ids</summary>
        RTSP_SESSION_ERROR,

        /// <summary>87 - unable to parse FTP file list</summary>
        FTP_BAD_FILE_LIST,

        /// <summary>88 - chunk callback reported error</summary>
        CHUNK_FAILED,

        /// <summary>89 - No connection available, the session will be queued</summary>
        NO_CONNECTION_AVAILABLE,

        /// <summary>90 - specified pinned public key did not match</summary>
        SSL_PINNEDPUBKEYNOTMATCH,

        /// <summary>91 - invalid certificate status</summary>
        SSL_INVALIDCERTSTATUS,

        /// <summary>92 - stream error in HTTP/2 framing layer</summary>
        HTTP2_STREAM,

        /// <summary>never use!</summary>
        CURL_LAST
    }
}