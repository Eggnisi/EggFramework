using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace EggFramework.DynamicText
{
    public class HtmlTagParser
    {
        public class TagNode
        {
            public string                     TagName       { get; set; }
            public Dictionary<string, object> Parameters    { get; } = new();
            public bool                       IsSelfClosing { get; set; }
            public string                     TextContent   { get; set; } // 用于 #TEXT 节点
            public List<TagNode>              Children      { get; } = new();
        }

        public static TagNode Parse(string input)
        {
            var parser = new HtmlTagParser(input);
            return parser.Parse();
        }

        private readonly string         _input;
        private          int            _position;
        private readonly Stack<TagNode> _tagStack = new Stack<TagNode>();

        private HtmlTagParser(string input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
        }

        private TagNode Parse()
        {
            var root = new TagNode { TagName = "ROOT" };
            _tagStack.Push(root);

            while (_position < _input.Length)
            {
                if (_input[_position] == '<')
                {
                    ProcessTag();
                }
                else
                {
                    CollectText();
                }
            }

            if (_tagStack.Count > 1)
            {
                throw new ParseException($"Unclosed tag: <{_tagStack.Peek().TagName}>", _position);
            }

            return root;
        }

        private void ProcessTag()
        {
            _position++; // Skip '<'
            if (_position >= _input.Length)
                throw new ParseException("Unexpected end after '<'", _position);

            // Handle closing tag
            if (_input[_position] == '/')
            {
                _position++; // Skip '/'
                string tagName = ReadTagName();
                SkipWhitespace();

                if (_position >= _input.Length || _input[_position] != '>')
                    throw new ParseException($"Expected '>' after closing tag </{tagName}>", _position);

                _position++;

                if (_tagStack.Count <= 1)
                    throw new ParseException($"Unexpected closing tag </{tagName}>", _position - tagName.Length - 3);

                var currentTag = _tagStack.Pop();

                if (currentTag.TagName != tagName)
                    throw new ParseException($"Mismatched tags: <{currentTag.TagName}> vs </{tagName}>", _position);
            }
            else
            {
                bool   isSelfClosing = false;
                string tagName       = ReadTagName();
                var    parameters    = new Dictionary<string, object>();
                
                SkipWhitespace();

                if (_position < _input.Length && _input[_position] == '=')
                {
                    _position++; 
                    SkipWhitespace();
                    
                    object paramValue = ReadParameterValue();
                    parameters["value"] = paramValue;
                }
                else
                {
                    while (_position < _input.Length &&
                           _input[_position] != '>' &&
                           _input[_position] != '/')
                    {
                        if (char.IsWhiteSpace(_input[_position]))
                        {
                            SkipWhitespace();
                            continue;
                        }
                        
                        if (_position >= _input.Length ||
                            _input[_position] == '>' ||
                            _input[_position] == '/')
                            break;
                        
                        var param = ReadParameter();
                        parameters[param.Key] = param.Value;
                        
                        SkipWhitespace();
                    }
                }

                // Check for self-closing tag
                if (_position < _input.Length && _input[_position] == '/')
                {
                    isSelfClosing = true;
                    _position++;
                }

                // Skip whitespace and expect '>'
                SkipWhitespace();
                if (_position >= _input.Length || _input[_position] != '>')
                    throw new ParseException($"Expected '>' after tag <{tagName}>", _position);

                _position++; // Skip '>'

                var tag = new TagNode
                {
                    TagName       = tagName,
                    IsSelfClosing = isSelfClosing
                };
                
                foreach (var param in parameters)
                {
                    tag.Parameters[param.Key] = param.Value;
                }

                _tagStack.Peek().Children.Add(tag);

                if (!isSelfClosing)
                {
                    _tagStack.Push(tag);
                }
            }
        }

        private void CollectText()
        {
            int start = _position;
            while (_position < _input.Length && _input[_position] != '<')
            {
                _position++;
            }

            if (_position > start)
            {
                string text = _input.Substring(start, _position - start);
                if (!string.IsNullOrEmpty(text))
                {
                    _tagStack.Peek().Children.Add(new TagNode
                    {
                        TagName     = "#TEXT",
                        TextContent = text
                    });
                }
            }
        }

        private string ReadTagName()
        {
            int start = _position;
            while (_position < _input.Length &&
                   !char.IsWhiteSpace(_input[_position]) &&
                   _input[_position] != '>' &&
                   _input[_position] != '/' &&
                   _input[_position] != '=')
            {
                _position++;
            }

            if (_position == start)
                throw new ParseException("Expected tag name after '<'", _position);

            return _input.Substring(start, _position - start);
        }

        private KeyValuePair<string, object> ReadParameter()
        {
            // Read parameter name
            int nameStart = _position;
            while (_position < _input.Length &&
                   !char.IsWhiteSpace(_input[_position]) &&
                   _input[_position] != '=' &&
                   _input[_position] != '>' &&
                   _input[_position] != '/')
            {
                _position++;
            }

            if (_position == nameStart)
                throw new ParseException("Expected parameter name", _position);

            string paramName = _input.Substring(nameStart, _position - nameStart);
            
            SkipWhitespace();
            
            if (_position >= _input.Length || _input[_position] != '=')
                throw new ParseException($"Expected '=' after parameter name '{paramName}'", _position);

            _position++; 
            
            SkipWhitespace();
            
            object paramValue = ReadParameterValue();

            return new KeyValuePair<string, object>(paramName, paramValue);
        }

        private object ReadParameterValue()
        {
            if (_position >= _input.Length)
                throw new ParseException("Unexpected end of input while reading parameter value", _position);

            char c = _input[_position];
            
            if (c is '"' or '\'')
            {
                return ReadQuotedString();
            }
            
            int start = _position;
            while (_position < _input.Length &&
                   !char.IsWhiteSpace(_input[_position]) &&
                   _input[_position] != '>' &&
                   _input[_position] != '/' &&
                   _input[_position] != '=')
            {
                _position++;
            }

            string valueStr = _input.Substring(start, _position - start);
            
            if (string.Equals(valueStr, "true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (string.Equals(valueStr, "false", StringComparison.OrdinalIgnoreCase))
                return false;
            
            if (int.TryParse(valueStr, NumberStyles.Integer, CultureInfo.InvariantCulture, out int intValue))
                return intValue;
            
            if (float.TryParse(valueStr, NumberStyles.Float, CultureInfo.InvariantCulture, out float floatValue))
                return floatValue;
            
            return valueStr;
        }

        private string ReadQuotedString()
        {
            char quoteChar = _input[_position];
            _position++; // Skip opening quote

            var sb = new StringBuilder();
            while (_position < _input.Length && _input[_position] != quoteChar)
            {
                if (_input[_position] == '\\')
                {
                    _position++; // Skip escape character
                    if (_position < _input.Length)
                    {
                        sb.Append(_input[_position]);
                        _position++;
                    }
                }
                else
                {
                    sb.Append(_input[_position]);
                    _position++;
                }
            }

            if (_position >= _input.Length)
                throw new ParseException("Unterminated quoted string", _position);

            _position++; // Skip closing quote
            return sb.ToString();
        }

        private void SkipWhitespace()
        {
            while (_position < _input.Length && char.IsWhiteSpace(_input[_position])) _position++;
        }
    }

    public class ParseException : Exception
    {
        public int Position { get; }

        public ParseException(string message, int position) : base($"{message} (at position {position})") =>
            Position = position;
    }
}